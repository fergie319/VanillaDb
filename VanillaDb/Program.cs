﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using VanillaDb.Models;
using VanillaDb.TypeTables;

namespace VanillaDb
{
    /// <summary>Main Program Class</summary>
    public class Program
    {
        private static ILogger Log { get; set; }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static int Main(string[] args)
        {
            // Set up and configure Serilog here
            Log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();
            Serilog.Log.Logger = Log;

            var result = 0;
            if (args.Length == 0)
            {
                throw new ArgumentException("<Table>.sql file must be supplied as the first argument.");
            }

            // First param is the table file - expect *.sql
            var sqlFileName = args[0];
            var sqlFileInfo = new FileInfo(sqlFileName);
            if (!sqlFileInfo.Exists)
            {
                throw new InvalidOperationException($"{sqlFileName} does not exist.");
            }

            // Open the file and start parsing
            var table = new TableModel()
            {
                Fields = new List<FieldModel>(),
            };
            var indexes = new List<IndexModel>();
            using (var stream = sqlFileInfo.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                // TODO: Use Microsoft.SqlServer.Management.SqlParser.Parser - for now it is too complicated
                //       Example Usage: https://stackoverflow.com/questions/30452864/parsing-t-sql-statements-to-tokens

                // NOTE: Parsing is inflexible, any deviation from expectations results in error

                // TODO: Add more detailed logging along the way.

                // First line should be table creation - extract table name (last parameter)
                var createTable = reader.ReadLine();
                var splitTableTokens = createTable.Split(new[] { "[", "]", ".", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitTableTokens.Length < 3)
                {
                    throw new InvalidOperationException("Create Table line should at least split into 3 parts: Create Table [name].");
                }

                // Start building the table model - starting with the name
                table.TableName = splitTableTokens.Last();

                // Second line should be just the parenthesis start
                if (reader.ReadLine() != "(")
                {
                    throw new InvalidOperationException("First line after Create Table statement should be just (");
                }

                // Now Loop and record each field until end of statement is reached ')'
                var fieldDef = reader.ReadLine();
                while (fieldDef != ")")
                {
                    var splitFieldTokens = fieldDef.Split(new[] { " ", "[", "]", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitFieldTokens.Length < 2)
                    {
                        throw new InvalidOperationException("Field definition line in Create Table statement should split into at least [FieldName] Type.");
                    }

                    var newField = new FieldModel()
                    {
                        FieldName = splitFieldTokens[0],
                        FieldType = ParseFieldType(splitFieldTokens[1]),
                        IsIdentity = splitFieldTokens.Any(s => string.Equals(s, "identity", StringComparison.InvariantCultureIgnoreCase)),
                        IsPrimaryKey = splitFieldTokens.Any(s => string.Equals(s, "primary", StringComparison.InvariantCultureIgnoreCase)),
                        IsNullable = fieldDef.IndexOf(" NOT NULL ", StringComparison.InvariantCultureIgnoreCase) == -1,
                    };
                    table.Fields.Add(newField);

                    // If this field is the primary key, then add an index for it
                    if (newField.IsPrimaryKey)
                    {
                        var index = new IndexModel()
                        {
                            Fields = new[] { newField }
                        };
                        indexes.Add(index);
                    }

                    // Break out of the loop if we've reached the end of the stream before encountering a )
                    if (reader.EndOfStream)
                    {
                        break;
                    }
                    else
                    {
                        fieldDef = reader.ReadLine();
                    }
                }

                // Now look for indexes on fields and record which fields are indexed
                if (!reader.EndOfStream)
                {
                    while (!reader.EndOfStream)
                    {
                        // First make sure this is an 'ON' clause (so part of Index definition)
                        var indexOnClause = reader.ReadLine().Trim();
                        if (indexOnClause.StartsWith("ON", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // The fields exist within the parenthesis, which are at the end of the line
                            var splitIndexTokens = indexOnClause.Trim().Split(new[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                            var fieldClause = splitIndexTokens.Last();

                            // Now split out the field names - removing whitespace characters
                            var splitFields = fieldClause.Split(new[] { ",", " ", "[", "]", "\t" }, StringSplitOptions.RemoveEmptyEntries);

                            // For each named field, find it add it to an index model
                            var index = new IndexModel()
                            {
                                Fields = new List<FieldModel>()
                            };
                            foreach (var fieldName in splitFields)
                            {
                                var field = table.Fields.FirstOrDefault(f => f.FieldName.Equals(fieldName));
                                if (field == null)
                                {
                                    throw new InvalidOperationException($"A field in an index was encountered that does not match a field in the table: {fieldName}");
                                }

                                index.Fields.Add(field);
                            }

                            indexes.Add(index);
                        }
                    }
                }
            }

            // TODO: First verify we're getting the models filled out.
            Log.Debug("Table: {@Table}", table);
            Log.Debug("Indexes: {@Indexes}", indexes);

            // Generate the stored procedures using our parsed table information
            // First generate type tables for all fields participating in indexes
            foreach (var index in indexes)
            {
                var typeTable = new TypeTable(index);
                var sqlContent = typeTable.TransformText();
                var fieldNames = string.Join("_", index.Fields.Select(f => f.FieldName));
                Log.Debug($"Output to: Types\\Type_{fieldNames}_Table.sql");
                Log.Debug($"Content: {sqlContent}");
                ////File.WriteAllText($"Types\\Type_{fieldNames}_Table.sql", sqlContent);
            }

            // Generate the C# classes and interfaces for working with the table and stored procedures

            // TODO: This is temporary for VS debugging - remove
            Console.ReadKey();

            return result;
        }

        /// <summary>Parses the type of the field from the given SQL Type markup.</summary>
        /// <param name="sqlTypeMarkup">The SQL type markup.</param>
        /// <returns>Field Type Model</returns>
        private static FieldTypeModel ParseFieldType(string sqlTypeMarkup)
        {
            // Initialize the return object
            var fieldType = new FieldTypeModel()
            {
                SqlType = sqlTypeMarkup,
                MaxLength = -1,
            };

            // Split the type into type+size for the char types that include size
            var splitFieldType = sqlTypeMarkup.Split(new[] { "(", ")", ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
            switch (splitFieldType[0].ToUpperInvariant())
            {
                case "VARCHAR":
                case "NVARCHAR":
                case "NCHAR":
                case "CHAR":
                    fieldType.FieldType = typeof(string);
                    if (splitFieldType.Length < 2)
                    {
                        throw new InvalidOperationException($"SQL Type is missing a length designation: {sqlTypeMarkup}");
                    }

                    if (int.TryParse(splitFieldType[1], out var maxLength))
                    {
                        fieldType.MaxLength = maxLength;
                    }
                    else if (string.Equals(splitFieldType[1], "MAX", StringComparison.InvariantCultureIgnoreCase))
                    {
                        fieldType.MaxLength = int.MaxValue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"{splitFieldType[1]} is not a valid length value for a SQL character type.");
                    }

                    break;
                case "INT":
                    fieldType.FieldType = typeof(int);
                    break;
            }

            return fieldType;
        }
    }
}

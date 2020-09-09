using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb
{
    /// <summary>Main Program Class</summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static int Main(string[] args)
        {
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
            using (var stream = sqlFileInfo.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                // TODO: Use Microsoft.SqlServer.Management.SqlParser.Parser - for now it is too complicated
                // TODO: Example Usage: https://stackoverflow.com/questions/30452864/parsing-t-sql-statements-to-tokens

                // NOTE: Parsing is inflexible, any deviation from expectations results in error

                // First line should be table creation - extract table name (last parameter)
                var createTable = reader.ReadLine();
                var splitTableTokens = createTable.Split(new[] { "[", "]", ".", " " }, StringSplitOptions.RemoveEmptyEntries);
                if (splitTableTokens.Length < 3)
                {
                    throw new InvalidOperationException("Create Table line should at least split into 3 parts: Create Table [name].");
                }

                // Start building the table model - starting with the name
                var table = new TableModel()
                {
                    TableName = splitTableTokens.Last(),
                    Fields = new List<FieldModel>(),
                };

                // Second line should be just the parenthesis start
                if (reader.ReadLine() != "(")
                {
                    throw new InvalidOperationException("First line after Create Table statement should be just (");
                }

                // Now Loop and record each field until end of statement is reached ')'
                var fieldDef = reader.ReadLine();
                while (fieldDef != ")")
                {
                    var splitFieldTokens = fieldDef.Split(new[] { " ", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitFieldTokens.Length < 2)
                    {
                        throw new InvalidOperationException("Field definition line in Create Table statement should split into at least [FieldName] Type.");
                    }

                    var newField = new FieldModel()
                    {
                        FieldName = splitFieldTokens[0],
                        SqlType = splitFieldTokens[1],
                        IsIdentity = splitFieldTokens.Any(s => string.Equals(splitFieldTokens[1], "identity", StringComparison.InvariantCultureIgnoreCase)),
                        IsPrimaryKey = splitFieldTokens.Any(s => string.Equals(splitFieldTokens[1], "primary", StringComparison.InvariantCultureIgnoreCase)),
                        IsNullable = fieldDef.IndexOf(" NOT NULL ", StringComparison.InvariantCultureIgnoreCase) == -1,
                    };

                    // TODO: Parse the type and convert it to a c# type
                    newField.FieldType = typeof(string);
                    newField.MaxLength = 10;

                    table.Fields.Add(newField);

                    // Break out of the loop if we've reached the end of the stream before encountering a )
                    if (reader.EndOfStream)
                    {
                        break;
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
                            var splitFields = fieldClause.Split(new[] { ",", " ", "[", "]" }, StringSplitOptions.RemoveEmptyEntries));

                            // For each named field, find it add it to an index model
                        }
                    }
                }
            }

            // Generate the stored procedures using our parsed table information

            // Generate the C# classes and interfaces for working with the table and stored procedures

            return result;
        }
    }
}

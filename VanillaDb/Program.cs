﻿using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VanillaDb.DataProviders;
using VanillaDb.DeleteProcs;
using VanillaDb.GetProcs;
using VanillaDb.InsertProcs;
using VanillaDb.Models;
using VanillaDb.TypeTables;
using VanillaDb.UpdateProcs;

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
            if (args.Length != 5)
            {
                throw new ArgumentException("Required Arguments: <Table>.sql|<directory> <outSqlDir> <outCodeDir> <namespace> <company-name>.");
            }

            // First param is the table file - expect *.sql - or directory containing *.sql table definition files
            var sqlFileName = args[0];
            var sqlFileInfo = new FileInfo(sqlFileName);
            var sqlDirectory = new DirectoryInfo(sqlFileName);
            if (!sqlFileInfo.Exists)
            {
                if (!sqlDirectory.Exists)
                {
                    throw new InvalidOperationException($"{sqlFileName} does not exist.");
                }
            }

            // Generate output in subdirectories of the table file directory's parent
            // In other words, we're assuming the table is in a Tables folder and want our output next to that folder
            var outputSqlDir = args[1];
            var outputCodeDir = args[2];
            var outputNamespace = args[3];
            var companyName = args[4];

            if (sqlFileInfo.Exists)
            {
                ProcessTableSql(sqlFileInfo, outputSqlDir, outputCodeDir, outputNamespace, companyName);
            }
            else if (sqlDirectory.Exists)
            {
                foreach (var fileInfo in sqlDirectory.EnumerateFiles("*.sql", SearchOption.AllDirectories))
                {
                    ProcessTableSql(fileInfo, outputSqlDir, outputCodeDir, outputNamespace, companyName);
                }
            }

            // Write out any static files
            File.WriteAllText($"{outputCodeDir}\\QueryOperator.cs",
@"
namespace " + outputNamespace + @"
{
    /// <summary>Enumeration for the different query operators available to use.</summary>
    public enum QueryOperator
    {
        /// <summary>The equals operator</summary>
        Equals = 0,
        /// <summary>The greater-than operator</summary>
        GreaterThan = 1,
        /// <summary>The less-than operator</summary>
        LessThan = 2
    }
}
");

            return result;
        }

        private static void ProcessTableSql(FileInfo sqlFileInfo, string outputSqlDir, string outputCodeDir, string outputNamespace, string companyName)
        {
            // Open the file and start parsing
            var table = new TableModel()
            {
                Company = companyName,
                Namespace = outputNamespace,
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

                // TODO: Add C# namespace input argument

                // First line should be table creation - extract table name (last parameter)
                var createTable = reader.ReadLine();
                var splitTableTokens = createTable.Split(new[] { "[", "]", ".", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitTableTokens.Length < 3)
                {
                    throw new InvalidOperationException("The first line must be the Create Table command, and it must split into a minimum of 3 parts: Create Table [name].");
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
                while (!fieldDef.StartsWith(")"))
                {
                    var splitFieldTokens = fieldDef.Split(new[] { " ", ",", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitFieldTokens.Length < 2)
                    {
                        throw new InvalidOperationException("Field definition line in Create Table statement should split into at least [FieldName] Type.");
                    }

                    // Make sure to remove any [ and ] characters for the field name
                    var fieldName = splitFieldTokens[0].Replace("[", string.Empty).Replace("]", string.Empty);
                    var newField = new FieldModel()
                    {
                        FieldName = fieldName,
                        IsIdentity = splitFieldTokens.Any(s => string.Equals(s, "identity", StringComparison.InvariantCultureIgnoreCase)),
                        IsPrimaryKey = splitFieldTokens.Any(s => string.Equals(s, "primary", StringComparison.InvariantCultureIgnoreCase)),
                        IsNullable = fieldDef.IndexOf(" NOT NULL", StringComparison.InvariantCultureIgnoreCase) == -1,
                    };
                    newField.FieldType = ParseFieldType(splitFieldTokens[1], newField.IsNullable);
                    table.Fields.Add(newField);

                    // If this field is the primary key, then add an index for it
                    if (newField.IsPrimaryKey)
                    {
                        var index = new IndexModel()
                        {
                            Table = table,
                            Fields = new[] { newField },
                            IsUnique = true,
                            IsPrimaryKey = true,
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
                        var nextLine = reader.ReadLine().Trim();
                        if (nextLine.StartsWith("CREATE", StringComparison.InvariantCultureIgnoreCase) &&
                            nextLine.IndexOf(" INDEX ", StringComparison.InvariantCultureIgnoreCase) > -1)
                        {
                            var isUnique = nextLine.IndexOf(" UNIQUE ", StringComparison.InvariantCultureIgnoreCase) > -1;
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
                                    Table = table,
                                    Fields = new List<FieldModel>(),
                                    IsUnique = isUnique
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
                            else
                            {
                                throw new InvalidOperationException("The CREATE INDEX statement must have the ON clause on the next line.");
                            }
                        }
                    }
                }
            }

            // Throw an error if the table has no primary key - no excuses!
            if (table.PrimaryKey == null)
            {
                throw new InvalidOperationException($"The table {table.TableName} doesn't have a primary key!  Fix this, you monster!");
            }

            // TODO: First verify we're getting the models filled out.
            Log.Debug("Table: {@Table}", table);
            Log.Debug("Indexes: {@Indexes}", indexes);

            // Create the output directories for all of the different files
            var typeTableDir = Path.Combine(outputSqlDir, "Types");
            Directory.CreateDirectory(typeTableDir);
            var storedProcDir = Path.Combine(outputSqlDir, $"Stored Procedures\\{table.TableName}");
            Directory.CreateDirectory(storedProcDir);

            // Generate the stored procedures using our parsed table information
            // First generate type tables for all fields participating in indexes
            foreach (var index in indexes)
            {
                var typeTable = new TypeTable(index);
                var sqlContent = typeTable.TransformText();
                var fieldNames = string.Join("_", index.Fields.Select(f => f.FieldName));
                Log.Debug($"Content: {sqlContent}");
                File.WriteAllText($"{typeTableDir}\\{typeTable.GenerateName()}.sql", sqlContent);

                // Generate the single-select stored procedures
                var getBy = new GetBySingleStoredProc(table, index);
                sqlContent = getBy.TransformText();
                Log.Debug($"Content: {sqlContent}");
                File.WriteAllText($"{storedProcDir}\\{getBy.GenerateName()}.sql", sqlContent);

                // Generate the bulk-select stored procedures
                var getByBulk = new GetByBulkStoredProc(table, index);
                sqlContent = getByBulk.TransformText();
                Log.Debug($"Content: {sqlContent}");
                File.WriteAllText($"{storedProcDir}\\{getByBulk.GenerateName()}.sql", sqlContent);
            }

            // Generate the Insert stored procedure
            var insertStoredProc = new InsertStoredProc(table);
            var insertContent = insertStoredProc.TransformText();
            Log.Debug($"Content: {insertContent}");
            File.WriteAllText($"{storedProcDir}\\{insertStoredProc.GenerateName()}.sql", insertContent);

            // Generate the Update stored procedure
            var updateStoredProc = new UpdateStoredProc(table);
            var updateContent = updateStoredProc.TransformText();
            Log.Debug($"Content: {updateContent}");
            File.WriteAllText($"{storedProcDir}\\{updateStoredProc.GenerateName()}.sql", updateContent);

            // Generate the Delete stored procedure
            var deleteStoredProc = new DeleteStoredProc(table);
            var deleteContent = deleteStoredProc.TransformText();
            Log.Debug($"Content: {deleteContent}");
            File.WriteAllText($"{storedProcDir}\\{deleteStoredProc.GenerateName()}.sql", deleteContent);

            // Generate the Bulk Delete stored procedure
            var deleteBulkStoredProc = new DeleteBulkStoredProc(table, indexes.First(i => i.IsPrimaryKey));
            var deleteBulkContent = deleteBulkStoredProc.TransformText();
            Log.Debug($"Content: {deleteBulkContent}");
            File.WriteAllText($"{storedProcDir}\\{deleteBulkStoredProc.GenerateName()}.sql", deleteBulkContent);

            // Create Data Provider directory
            var dataProviderDir = Path.Combine(outputCodeDir, table.TableName);
            Directory.CreateDirectory(dataProviderDir);

            // Generate the DataModel class
            var dataModelGen = new DataModel(table);
            var content = dataModelGen.TransformText();
            Log.Debug($"Content: {content}");
            File.WriteAllText($"{dataProviderDir}\\{dataModelGen.GenerateName()}.cs", content);

            // Generate the DataProvider interface
            var dataProviderInterfaceGen = new DataProviderInterface(table, indexes);
            content = dataProviderInterfaceGen.TransformText();
            Log.Debug($"Content: {content}");
            File.WriteAllText($"{dataProviderDir}\\{dataProviderInterfaceGen.GenerateName()}.cs", content);

            // Generate the SqlDataProvider class
            var sqlDataProviderGen = new SqlDataProvider(table, indexes);
            content = sqlDataProviderGen.TransformText();
            Log.Debug($"Content: {content}");
            File.WriteAllText($"{dataProviderDir}\\{sqlDataProviderGen.GenerateName()}.cs", content);
        }

        /// <summary>Parses the type of the field from the given SQL Type markup.</summary>
        /// <param name="sqlTypeMarkup">The SQL type markup.</param>
        /// <param name="isNullable">Indicates whether the type is nullable</param>
        /// <returns>Field Type Model</returns>
        private static FieldTypeModel ParseFieldType(string sqlTypeMarkup, bool isNullable)
        {
            // Initialize the return object
            var fieldType = new FieldTypeModel()
            {
                SqlType = sqlTypeMarkup.Replace("[", string.Empty).Replace("]", string.Empty),
                MaxLength = -1,
                IsNullable = isNullable
            };

            // Split the type into type+size for the char types that include size
            var splitFieldType = sqlTypeMarkup.Split(new[] { "[", "]", "(", ")", ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
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
                case "TINYINT":
                case "SMALLINT":
                    fieldType.FieldType = typeof(int);
                    break;
                case "BIGINT":
                    fieldType.FieldType = typeof(long);
                    break;
                case "NUMERIC":
                case "REAL":
                case "FLOAT":
                    fieldType.FieldType = typeof(double);
                    break;
                case "DECIMAL":
                case "MONEY":
                    fieldType.FieldType = typeof(decimal);
                    break;
                case "BIT":
                    fieldType.FieldType = typeof(bool);
                    break;
                case "DATE":
                case "DATETIME":
                case "DATETIME2":
                case "TIME":
                case "SMALLDATETIME":
                    fieldType.FieldType = typeof(DateTime);
                    break;
            }

            return fieldType;
        }
    }
}

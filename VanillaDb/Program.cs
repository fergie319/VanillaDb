using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using VanillaDb.Configuration;
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
        /// <summary>The configuration file name</summary>
        public const string ConfigFileName = "vanillaDb.config";

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
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            VanillaConfig config = null;
            if (args.Length == 0)
            {
                // If no arguments, walk up parent directories looking for a vanillaDb.config file
                var configFileInfo = new FileInfo(Path.Combine(currentDirectory.FullName, ConfigFileName));
                while (configFileInfo != null && !configFileInfo.Exists)
                {
                    var parentDirectory = configFileInfo.Directory.Parent;
                    if (parentDirectory != null)
                    {
                        configFileInfo = new FileInfo(Path.Combine(configFileInfo.Directory.Parent.FullName, ConfigFileName));
                    }
                    else
                    {
                        configFileInfo = null;
                    }
                }

                if (configFileInfo == null)
                {
                    throw new InvalidOperationException(
                        $"{ConfigFileName} is required in folder hierarchy, or all required arguments must be provided." +
                        "Required Arguments: <Table>.sql|<directory> <outSqlDir> <outCodeDir> <namespace> <company-name> (optional)--generate-config <config-path>.");
                }
                else
                {
                    // Set the working directory to the vanillaDb.config folder so relative paths work relative to that file
                    Directory.SetCurrentDirectory(configFileInfo.Directory.FullName);
                    var configString = File.ReadAllText(configFileInfo.FullName);
                    config = JsonConvert.DeserializeObject<VanillaConfig>(configString);
                }
            }
            else if (args.Length == 5 || args.Length == 7)
            {
                config = new VanillaConfig()
                {
                    TableSqlPath = args[0],
                    OutputSqlPath = args[1],
                    OutputCodePath = args[2],
                    CodeNamespace = args[3],
                    CompanyName = args[4]
                };

                // If 6 arguments, then check for --generate-config
                if (args.Length == 7 && string.Equals(args[5], "--generate-config", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Set the working directory to the vanillaDb.config folder so relative paths work relative to that file
                    var outputLocation = args[6];
                    var outputContents = JsonConvert.SerializeObject(config, Formatting.Indented);
                    var outputDirInfo = new DirectoryInfo(args[6]);
                    Directory.SetCurrentDirectory(outputDirInfo.FullName);
                    File.WriteAllText(Path.Combine(outputLocation, ConfigFileName), outputContents);
                }
            }
            else if (args.Length != 5)
            {
                // If 5 arguments, then assume all arguments were provided
                throw new ArgumentException(
                        $"{ConfigFileName} is required in folder hierarchy, or all required arguments must be provided." +
                        "Required Arguments: <Table>.sql|<directory> <outSqlDir> <outCodeDir> <namespace> <company-name> (optional)--generate-config <config-path>.");
            }

            // First param is the table file - expect *.sql - or directory containing *.sql table definition files
            var sqlFileInfo = new FileInfo(config.TableSqlPath);
            var sqlDirectory = new DirectoryInfo(config.TableSqlPath);

            if (!sqlFileInfo.Exists)
            {
                if (!sqlDirectory.Exists)
                {
                    throw new InvalidOperationException($"{config.TableSqlPath} does not exist.");
                }
            }

            // Process just the table file, or all files in the folder depending on what was configured
            if (sqlFileInfo.Exists)
            {
                ProcessTableSql(sqlFileInfo, config);
            }
            else if (sqlDirectory.Exists)
            {
                foreach (var fileInfo in sqlDirectory.EnumerateFiles("*.sql", SearchOption.AllDirectories))
                {
                    ProcessTableSql(fileInfo, config);
                }
            }

            // Write out any static files
            File.WriteAllText($"{config.OutputCodePath}\\QueryOperator.cs",
@"
namespace " + config.CodeNamespace + @"
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

        private static void ProcessTableSql(FileInfo sqlFileInfo, VanillaConfig config)
        {
            // Open the file and start parsing
            var table = new TableModel()
            {
                Company = config.CompanyName,
                Namespace = config.CodeNamespace,
                Fields = new List<FieldModel>(),
                Config = new TableConfig()
                {
                    GetAll = true,
                    Insert = true,
                    Update = true,
                    Delete = true
                }
            };
            var indexes = new List<IndexModel>();
            using (var stream = sqlFileInfo.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                // TODO: Use Microsoft.SqlServer.Management.SqlParser.Parser - for now it is too complicated
                //       Example Usage: https://stackoverflow.com/questions/30452864/parsing-t-sql-statements-to-tokens

                // NOTE: Parsing is inflexible, any deviation from expectations results in error

                // TODO: Add more detailed logging along the way.

                // Check first line for /* and grab every line until encountering */ to parse as table configuration
                var firstLine = ReadLineOfSql(reader);
                var createTable = string.Empty;
                if (firstLine.Trim() == "/*")
                {
                    // Read all the contents of the block comment to build the table config JSON
                    var line = ReadLineOfSql(reader);
                    var tableConfigJson = string.Empty;
                    while (line.Trim() != "*/")
                    {
                        tableConfigJson += line;
                        line = ReadLineOfSql(reader);
                    }

                    // Deserialize the table config and then read the next line (should be Create Table)
                    table.Config = JsonConvert.DeserializeObject<TableConfig>(tableConfigJson);
                    createTable = ReadLineOfSql(reader);
                }
                else
                {
                    createTable = firstLine;
                }

                // Ignore any blank lines before encountering the Create Table method
                while (string.IsNullOrEmpty(createTable.Trim()))
                {
                    createTable = ReadLineOfSql(reader);
                }

                // extract table name from the createTable line (last parameter)
                var splitTableTokens = createTable.Split(new[] { "[", "]", ".", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitTableTokens.Length < 3)
                {
                    throw new InvalidOperationException("The first line must be the Create Table command, and it must split into a minimum of 3 parts: Create Table [name].");
                }

                // Start building the table model - starting with the name and the schema
                table.TableName = splitTableTokens.Last();
                table.Schema = splitTableTokens.Reverse().Skip(1).Take(1).FirstOrDefault() ?? throw new InvalidOperationException("Missing Schema from Table Name");

                // Second line should be just the parenthesis start
                if (ReadLineOfSql(reader) != "(")
                {
                    throw new InvalidOperationException("First line after Create Table statement should be just (");
                }

                // Now Loop and record each field until end of statement is reached ')'
                var fieldDef = ReadLineOfSql(reader);
                while (!fieldDef.StartsWith(")"))
                {
                    // Remove all trailing commas, as they are useless and will only confuse further parsing
                    fieldDef = fieldDef.TrimEnd(new[] { ',' });
                    var splitFieldTokens = fieldDef.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitFieldTokens.Length < 2)
                    {
                        throw new InvalidOperationException("Field definition line in Create Table statement should split into at least [FieldName] Type.");
                    }

                    // Check for keywords - skip this line if one is encountered (only checking for PERIOD for now)
                    var rawFieldName = splitFieldTokens[0];
                    if (rawFieldName.Equals("period", StringComparison.InvariantCultureIgnoreCase))
                    {
                        fieldDef = ReadLineOfSql(reader);
                        continue;
                    }

                    // Make sure to remove any [ and ] characters for the field name
                    var fieldName = rawFieldName.Replace("[", string.Empty).Replace("]", string.Empty);
                    var newField = new FieldModel()
                    {
                        FieldName = fieldName,
                        IsIdentity = splitFieldTokens.Any(s => (s.StartsWith("identity", StringComparison.InvariantCultureIgnoreCase))),
                        IsPrimaryKey = splitFieldTokens.Any(s => string.Equals(s, "primary", StringComparison.InvariantCultureIgnoreCase)),
                        IsNullable = fieldDef.IndexOf(" NOT NULL", StringComparison.InvariantCultureIgnoreCase) == -1,
                    };

                    if (fieldDef.IndexOf("datetime", StringComparison.InvariantCultureIgnoreCase) > -1 &&
                        fieldDef.IndexOf("generated", StringComparison.InvariantCultureIgnoreCase) > -1 &&
                        fieldDef.IndexOf("row", StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        newField.IsTemporalField = true;
                        newField.IsTemporalStart = fieldDef.IndexOf("row start", StringComparison.InvariantCultureIgnoreCase) != -1;
                        newField.IsTemporalEnd = fieldDef.IndexOf("row end", StringComparison.InvariantCultureIgnoreCase) != -1;
                    }

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
                        fieldDef = ReadLineOfSql(reader);
                    }
                }

                // Now look for indexes on fields and record which fields are indexed
                if (!reader.EndOfStream)
                {
                    while (!reader.EndOfStream)
                    {
                        // First make sure this is an 'ON' clause (so part of Index definition)
                        var nextLine = ReadLineOfSql(reader);
                        if (nextLine.StartsWith("CREATE", StringComparison.InvariantCultureIgnoreCase) &&
                            nextLine.IndexOf(" INDEX ", StringComparison.InvariantCultureIgnoreCase) > -1)
                        {
                            var isUnique = nextLine.IndexOf(" UNIQUE ", StringComparison.InvariantCultureIgnoreCase) > -1;
                            var indexOnClause = ReadLineOfSql(reader);
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
                        else if (nextLine.StartsWith("WITH", StringComparison.InvariantCultureIgnoreCase) &&
                                 nextLine.ToUpper().Contains("SYSTEM_VERSIONING = ON"))
                        {
                            table.IsTemporal = true;
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
            var typeTableDir = Path.Combine(config.OutputSqlPath, "Types");
            Directory.CreateDirectory(typeTableDir);
            var storedProcDir = Path.Combine(config.OutputSqlPath, $"Stored Procedures\\{table.TableName}");
            Directory.CreateDirectory(storedProcDir);

            // Generate the stored procedures using our parsed table information
            if (table.Config.GetAll)
            {
                // Generate the single-select stored procedures
                var getByAll = new GetAllStoredProc(table);
                var sqlContent = getByAll.TransformText();
                Log.Debug($"Content: {sqlContent}");
                File.WriteAllText($"{storedProcDir}\\{getByAll.GenerateName()}.sql", sqlContent);
            }

            // First generate type tables for all fields participating in indexes and then the GetBy procs
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
            if (table.Config.Insert)
            {
                var insertStoredProc = new InsertStoredProc(table);
                var insertContent = insertStoredProc.TransformText();
                Log.Debug($"Content: {insertContent}");
                File.WriteAllText($"{storedProcDir}\\{insertStoredProc.GenerateName()}.sql", insertContent);
            }

            // Generate the Update stored procedure
            if (table.Config.Update)
            {
                var updateStoredProc = new UpdateStoredProc(table);
                var updateContent = updateStoredProc.TransformText();
                Log.Debug($"Content: {updateContent}");
                File.WriteAllText($"{storedProcDir}\\{updateStoredProc.GenerateName()}.sql", updateContent);
            }

            if (table.Config.Delete)
            {
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
            }

            // Create Data Provider directory
            var dataProviderDir = Path.Combine(config.OutputCodePath, table.TableName);
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

            // Overwrite the table.sql file to include the table config at the beginning
            var fileContent = string.Empty;
            using (var stream = sqlFileInfo.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                fileContent = reader.ReadLine();
                var createTable = string.Empty;
                if (fileContent.Trim() == "/*")
                {
                    // Read all the contents of the block comment to build the table config JSON
                    var line = reader.ReadLine();
                    while (line.Trim() != "*/")
                    {
                        line = reader.ReadLine();
                    }

                    // Read the next line (should be Create Table)
                    fileContent = reader.ReadLine();
                }

                // Read the remainder of the file
                fileContent += Environment.NewLine + reader.ReadToEnd();

                // Serialize the TableConfig JSON into a comment block
                var newLine = Environment.NewLine;
                var tableConfigJson = JsonConvert.SerializeObject(table.Config, Formatting.Indented);
                fileContent = $"/*{newLine}{tableConfigJson}{newLine}*/{newLine}{fileContent}";
            }

            File.WriteAllText(sqlFileInfo.FullName, fileContent);
        }

        /// <summary>Reads the next line of text from the given TextReader and automatically trims whitespace and strips out line comments.</summary>
        /// <param name="reader">The text reader.</param>
        /// <returns>The next line of clean sql from the TextReader.</returns>
        private static string ReadLineOfSql(TextReader reader)
        {
            // First read the next line
            var nextLine = reader.ReadLine();

            // Look for -- comments and trim them
            var commentStartIndex = nextLine.IndexOf("--");
            if (commentStartIndex > -1)
            {
                nextLine = nextLine.Substring(0, commentStartIndex);
            }

            // Trim whitespace before returning
            nextLine = nextLine.Trim();
            return nextLine;
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
                    fieldType.FieldType = typeof(int);
                    break;
                case "SMALLINT":
                    fieldType.FieldType = typeof(short);
                    break;
                case "TINYINT":
                    fieldType.FieldType = typeof(byte);
                    break;
                case "BIGINT":
                    fieldType.FieldType = typeof(long);
                    break;
                case "REAL":
                case "FLOAT":
                    fieldType.FieldType = typeof(double);
                    break;
                case "NUMERIC":
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

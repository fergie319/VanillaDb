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

        /// <summary>Gets or sets a value indicating whether console output should be verbose.</summary>
        private static bool IsVerbose { get; set; }

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

            // Check for the "verbose" argument and remove it now so further parsing works correctly
            if (args.Any(arg => string.Equals(arg, "--verbose", StringComparison.InvariantCultureIgnoreCase)))
            {
                IsVerbose = true;
                args = args.Where(arg => !string.Equals(arg, "--verbose", StringComparison.InvariantCultureIgnoreCase))
                           .ToArray();
            }

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
                        $"{ConfigFileName} is required in folder hierarchy, or all required arguments must be provided.\n" +
                        "Required Arguments: <Table>.sql|<directory> <outSqlDir> <outCodeDir> <namespace> <company-name> (optional)--generate-config <config-path>.\n" +
                        "Optional Arguments: --verbose");
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

            File.WriteAllText($"{config.OutputCodePath}\\TemporalTypes.cs",
@"
namespace " + config.CodeNamespace + @"
{
    /// <summary>Enumeration for the types of temporal queries supported by VanillaDb</summary>
    public enum TemporalTypes
    {
        /// <summary>The default behavior - for querying current record.</summary>
        Default = 0,
        /// <summary>As of - for querying a record(s) as of a specific date.</summary>
        AsOf = 1,
        /// <summary>All - for querying all current and historical data.</summary>
        All = 2
    }
}
");

            return result;
        }

        private static void ProcessTableSql(FileInfo tableFileInfo, VanillaConfig config)
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
            using (var stream = tableFileInfo.OpenRead())
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

                // Start building the table model - starting with the name, alias, and the schema
                table.TableName = splitTableTokens.Last();
                table.Schema = splitTableTokens.Reverse().Skip(1).Take(1).FirstOrDefault() ?? throw new InvalidOperationException("Missing Schema from Table Name");
                if (string.IsNullOrWhiteSpace(table.Config.TableAlias))
                {
                    table.Config.TableAlias = table.TableName
                                                .Replace("_", string.Empty)
                                                .Replace("-", string.Empty)
                                                .Replace("tbl", string.Empty);
                }

                // Second line should be just the parenthesis start
                if (ReadLineOfSql(reader) != "(")
                {
                    throw new InvalidOperationException($"{table.TableName} - First line after Create Table statement should be just (");
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

                    // If this is a primary key constraint, then attempt to parse out the index and continue
                    if (rawFieldName.Equals("constraint", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (fieldDef.IndexOf("primary key", StringComparison.InvariantCultureIgnoreCase) > -1)
                        {
                            // The fields exist within the parenthesis, which are at the end of the line
                            var splitIndexTokens = fieldDef.Trim(new[] { ',', ' ' }).Split(new[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                            var fieldClause = splitIndexTokens.Last();

                            // Now split out the field names - removing whitespace characters
                            var splitFields = fieldClause.Split(new[] { ",", " ", "[", "]", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                            var fields = table.Fields.Where(f => splitFields.Any(sf => string.Equals(f.FieldName, sf, StringComparison.InvariantCultureIgnoreCase))).ToList();

                            var index = new IndexModel()
                            {
                                Table = table,
                                Fields = fields,
                                IsUnique = true,
                                IsPrimaryKey = true,
                            };
                            indexes.Add(index);
                        }
                        else
                        {
                            Log.Warning("A non-primary key constraint was encountered - this will be ignored by VanillaDb - use different syntax " +
                                        "(like a created index, or the UNIQUE keyword in the field definition) if you expect generated methods for it.");
                        }

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

                    // Make sure that the field is marked NOT nullable if it is a Primary Key or an Identity column
                    // (if not explicitly stated as NULL)
                    if (newField.IsPrimaryKey ||
                        newField.IsIdentity && fieldDef.IndexOf("NULL", StringComparison.InvariantCultureIgnoreCase) == -1)
                    {
                        newField.IsNullable = false;
                    }

                    if (fieldDef.IndexOf("datetime", StringComparison.InvariantCultureIgnoreCase) > -1 &&
                        fieldDef.IndexOf("generated", StringComparison.InvariantCultureIgnoreCase) > -1 &&
                        fieldDef.IndexOf("row", StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        newField.IsTemporalField = true;
                        newField.IsTemporalStart = fieldDef.IndexOf("row start", StringComparison.InvariantCultureIgnoreCase) != -1;
                        newField.IsTemporalEnd = fieldDef.IndexOf("row end", StringComparison.InvariantCultureIgnoreCase) != -1;
                    }

                    // Get the field type and grab the following token if it is incomplete - like "Decimal(18," - where we need the rest of the type
                    // Future Matt: If you find yourself looking at this code because it's not parsing correctly, then just write code at the beginning
                    //              of the parsing to find the open/close parenthesis and remove all spaces within them.
                    var fieldTypeString = splitFieldTokens[1];
                    if (fieldTypeString.EndsWith(","))
                    {
                        fieldTypeString += splitFieldTokens[2];
                    }

                    newField.FieldType = ParseFieldType(fieldTypeString, newField.IsNullable);
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

            // Throw an error if the table has no primary key - no excuses!  Well, except that we have an option now...
            if (table.PrimaryKey == null)
            {
                // If the table has no primary key setting, but also doesn't have a primary key, then
                // default it to false and write out the table so that the user can switch it.
                if (!table.Config.AllowNoPrimaryKey.HasValue)
                {
                    table.Config.AllowNoPrimaryKey = false;
                    WriteTableConfigToFile(tableFileInfo, table);
                }

                if (!table.Config.AllowNoPrimaryKey.Value)
                {
                    throw new InvalidOperationException(
                        $"The table {table.TableName} doesn't have a primary key!  Fix this, you monster!  " +
                        "Alternatively, if you wish to remain a monster, you can change the AllowNoPrimaryKey " +
                        "setting that has been added to your table config to be 'true'.  However, I strongly " +
                        "discourage this, and am judging you.");
                }
                else
                {
                    // If we are allowing the table to not have a primary key, then explicitly disable
                    // Update and Delete because those require a primary key to match on.
                    table.Config.Update = false;
                    table.Config.Delete = false;
                }
            }

            // If this is a temporal table, then set the default config if not already provided
            if (table.IsTemporal)
            {
                table.Config.TemporalGetAll = table.Config.TemporalGetAll ?? true;
                table.Config.TemporalGetAsOf = table.Config.TemporalGetAsOf ?? true;
            }

            // Log the table and indexes if verbose is enabled
            if (IsVerbose)
            {
                Log.Debug("Table: {@Table}", table);
                Log.Debug("Indexes: {@Indexes}", indexes);
            }

            // Create the output directories for all of the different files
            var typeTableDir = Path.Combine(config.OutputSqlPath, "Types");
            Directory.CreateDirectory(typeTableDir);
            var storedProcDir = Path.Combine(config.OutputSqlPath, $"Stored Procedures\\{table.TableAlias}");
            Directory.CreateDirectory(storedProcDir);

            // Generate the stored procedures using our parsed table information
            if (table.Config.GetAll)
            {
                // Generate the single-select stored procedures
                var getByAll = new GetAllStoredProc(table);
                getByAll.GenerateFile(storedProcDir);
            }

            // First generate type tables for all fields participating in indexes and then the GetBy procs
            foreach (var index in indexes)
            {
                var typeTable = new TypeTable(index);
                typeTable.GenerateFile(typeTableDir);

                // Generate the single-select stored procedures
                var getBy = new GetBySingleStoredProc(table, index);
                getBy.GenerateFile(storedProcDir);

                // Generate the bulk-select stored procedures
                var getByBulk = new GetByBulkStoredProc(table, index);
                getByBulk.GenerateFile(storedProcDir);

                if (table.IsTemporal)
                {
                    if (table.Config.TemporalGetAll.HasValue && table.Config.TemporalGetAll.Value)
                    {
                        // Generate the single-select stored procedures
                        getBy = new GetBySingleStoredProc(table, index, TemporalTypes.All);
                        getBy.GenerateFile(storedProcDir);

                        // Generate the bulk-select stored procedures
                        getByBulk = new GetByBulkStoredProc(table, index, TemporalTypes.All);
                        getByBulk.GenerateFile(storedProcDir);
                    }

                    if (table.Config.TemporalGetAsOf.HasValue && table.Config.TemporalGetAsOf.Value)
                    {
                        // Generate the single-select stored procedures
                        getBy = new GetBySingleStoredProc(table, index, TemporalTypes.AsOf);
                        getBy.GenerateFile(storedProcDir);

                        // Generate the bulk-select stored procedures
                        getByBulk = new GetByBulkStoredProc(table, index, TemporalTypes.AsOf);
                        getByBulk.GenerateFile(storedProcDir);
                    }
                }
            }

            // Generate the Insert stored procedure
            if (table.Config.Insert)
            {
                var insertStoredProc = new InsertStoredProc(table);
                insertStoredProc.GenerateFile(storedProcDir);
            }

            // Generate the Update stored procedure
            if (table.Config.Update)
            {
                var updateStoredProc = new UpdateStoredProc(table);
                updateStoredProc.GenerateFile(storedProcDir);
            }

            if (table.Config.Delete)
            {
                // Generate the Delete stored procedure
                var deleteStoredProc = new DeleteStoredProc(table);
                deleteStoredProc.GenerateFile(storedProcDir);

                // Generate the Bulk Delete stored procedure
                var deleteBulkStoredProc = new DeleteBulkStoredProc(table, indexes.First(i => i.IsPrimaryKey));
                deleteBulkStoredProc.GenerateFile(storedProcDir);
            }

            // Create Data Provider directory
            var dataProviderDir = Path.Combine(config.OutputCodePath, table.TableAlias);
            Directory.CreateDirectory(dataProviderDir);

            // Generate the DataModel class
            var dataModelGen = new DataModel(table);
            dataModelGen.GenerateFile(dataProviderDir);

            // Generate the DataProvider interface
            var dataProviderInterfaceGen = new DataProviderInterface(table, indexes);
            dataProviderInterfaceGen.GenerateFile(dataProviderDir);

            // Generate the SqlDataProvider classC:\git\RFA.Bank.Finance\Remittance.Database\Tables\pSubledger_Daily.sql
            var sqlDataProviderGen = new SqlDataProvider(table, indexes);
            sqlDataProviderGen.GenerateFile(dataProviderDir);

            // Check if a datadictionary exists for the table
            var extendedPropertiesDir = Path.Combine(config.OutputSqlPath, $"Extended Properties");
            var extendedPropertiesFile = Path.Combine(extendedPropertiesDir, $"{table.TableAlias}.sql");
            var dictionaryFilePath = Path.Combine(tableFileInfo.DirectoryName, $"{table.TableName}.csv");
            var dictionaryFileInfo = new FileInfo(dictionaryFilePath);
            if (dictionaryFileInfo.Exists)
            {
                var extendedPropertiesScript =
                    DataDictionaryParser.ProcessDataDictionary(
                        dictionaryFileInfo,
                        table.Schema,
                        table.TableName);

                Directory.CreateDirectory(extendedPropertiesDir);
                File.WriteAllText(extendedPropertiesFile, extendedPropertiesScript);
            }

            // Add table config to file for easy configuration
            WriteTableConfigToFile(tableFileInfo, table);
        }

        private static void WriteTableConfigToFile(FileInfo tableFileInfo, TableModel table)
        {
            // Overwrite the table.sql file to include the table config at the beginning
            var fileContent = string.Empty;
            using (var stream = tableFileInfo.OpenRead())
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

            File.WriteAllText(tableFileInfo.FullName, fileContent);
        }

        /// <summary>Logs the given message when Verbose is enabled.</summary>
        /// <param name="message">The message.</param>
        /// <param name="arguments">The arguments.</param>
        public static void LogVerbose(string message, params string[] arguments)
        {
            if (IsVerbose)
            {
                Log.Debug(message, arguments);
            }
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

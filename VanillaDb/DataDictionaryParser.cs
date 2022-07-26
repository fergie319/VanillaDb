using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using VanillaDb.Models;

namespace VanillaDb
{

    /// <summary>Parses data dictionary CSV to create SQL Server Extended Properties.</summary>
    public static class DataDictionaryParser
    {
        /// <summary>
        /// Accept Data Dictionary CSV and generate extended property sql script
        /// </summary>
        /// <param name="file"></param>
        /// <param name="schemaName"></param>
        /// <param name="tableName"></param>
        /// <remarks>the indexs supplied should be 0 index based columns in csv.</remarks>
        public static string ProcessDataDictionary(FileInfo file, string schemaName, string tableName)
        {
            file = file ?? throw new ArgumentNullException(nameof(file));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                Encoding = Encoding.UTF8,
                HasHeaderRecord = true,
                MissingFieldFound = null
            };
            using (var reader = new StreamReader(file.OpenRead(), Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var definitionDictionary = new Dictionary<string, string>();
                var valuesDictionary = new Dictionary<string, string>();

                csv.Read();
                csv.ReadHeader();
                var definitions = csv.GetRecords<DictionaryDefinitionModel>().ToArray();

                // TODO: Update the 'GenerateExtendedPropertyScript method to accept the DictionaryDefinitionModel enumerable
                var returnScript = GenerateExtendedPropertyScript("MS_Description", definitionDictionary, schemaName, tableName);
                returnScript += GenerateExtendedPropertyScript("ENUM_Definition", valuesDictionary, schemaName, tableName);

                return returnScript;
            }
        }

        private static string GenerateExtendedPropertyScript(string name, Dictionary<string, string> dictionary, string schema, string table)
        {
            var returnString = string.Empty;

            foreach (var kvp in dictionary)
            {
                returnString +=
                $@"EXEC sp_addextendedproperty
            @name = '{name}', @value = '{kvp.Value}',
            @level0type = N'Schema', @level0name = '{schema}',
            @level1type = N'Table', @level1name = '{table}',
            @level2type = N'Column', @level2name = '{kvp.Key}'";

                returnString += Environment.NewLine;
            }

            return returnString;
        }
    }
}

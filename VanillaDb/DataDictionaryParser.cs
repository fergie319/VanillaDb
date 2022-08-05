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
        /// <summary>Parses CSV from the given file to extract a data dictionary - matching DictionaryDefinitionModel.</summary>
        /// <param name="file">The file to parse.</param>
        /// <returns>Enumerable of DictionaryDefinitionModel</returns>
        /// <remarks>the indexs supplied should be 0 index based columns in csv.</remarks>
        public static IEnumerable<DictionaryDefinitionModel> ParseDataDictionary(FileInfo file)
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
                csv.Read();
                csv.ReadHeader();
                var definitions = csv.GetRecords<DictionaryDefinitionModel>().ToArray();
                return definitions;
            }
        }

        /// <summary>Generates the extended property script from the given data dictionary for the given schema.table.</summary>
        /// <param name="dictionary">The data dictionary.</param>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="table">The table name.</param>
        /// <returns></returns>
        public static string GenerateExtendedPropertyScript(IEnumerable<DictionaryDefinitionModel> dictionary, string schema, string table)
        {
            var returnString = string.Empty;
            var extendedPropertyScript =
                @"EXEC sp_addextendedproperty
            @name = '{0}', @value = '{1}',
            @level0type = N'Schema', @level0name = '{2}',
            @level1type = N'Table', @level1name = '{3}',
            @level2type = N'Column', @level2name = '{4}'" +
            Environment.NewLine + "GO;" + Environment.NewLine;

            foreach (var definition in dictionary)
            {
                var description = definition.Description.Replace("\r", " ").Replace("\n", " ").Replace("'", "''");
                returnString += string.Format
                    (extendedPropertyScript, "MS_Description", description, schema, table, definition.Field);
                if (!string.IsNullOrEmpty(definition.Values))
                {
                    returnString += string.Format
                        (extendedPropertyScript, "ENUM_Definition", definition.ParsedValues, schema, table, definition.Field);
                }
            }

            return returnString;
        }
    }
}

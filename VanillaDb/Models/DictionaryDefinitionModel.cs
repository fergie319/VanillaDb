using System;
using CsvHelper.Configuration.Attributes;

namespace VanillaDb.Models
{
    /// <summary>Represents the expected CSV for specifying Data Dictionaries for a table.</summary>
    public class DictionaryDefinitionModel
    {
        /// <summary>Gets or sets the field.</summary>
        [Name("Field")]
        public string Field { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [Name("Description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the values.</summary>
        [Name("Values")]
        public string Values { get; set; }

        /// <summary>Gets the values - but parsed so that values are separated by '|' characters.</summary>
        /// <value>The parsed values.</value>
        public string ParsedValues
        {
            get
            {
                var result = Values.Replace("'", "''");
                if (!string.IsNullOrEmpty(Values))
                {
                    var splitValues = Values.Split(new[] { '\r', '\n', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    result = string.Join("|", splitValues);
                }

                return result;
            }
        }
    }
}

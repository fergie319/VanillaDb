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
    }
}

using Newtonsoft.Json;

namespace VanillaDb.Configuration
{
    /// <summary>Stores VanillaDb table generation configuration settings</summary>
    public class TableConfig
    {
        /// <summary>Gets or sets a value indicating whether GetAll endpoints should be generated.</summary>
        public bool GetAll { get; set; }

        /// <summary>Gets or sets a value indicating whether Insert endpoints should be generated.</summary>
        public bool Insert { get; set; }

        /// <summary>Gets or sets a value indicating whether Update endpoints should be generated.</summary>
        public bool Update { get; set; }

        /// <summary>Gets or sets a value indicating whether Delete endpoints should be generated.</summary>
        public bool Delete { get; set; }

        /// <summary>Gets or sets whether this table is allowed to have no primary key.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowNoPrimaryKey { get; set; }

        /// <summary>Gets or sets the table alias - used for naming models, dataproviders, and interfaces.</summary>
        public string TableAlias { get; set; }

        /// <summary>Gets or sets a value indicating whether the temporal endpoints for getting all history should be generated.</summary>
        /// <remarks>This is nullable so that it won't be rendered for non-temporal tables</remarks>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? TemporalGetAll { get; set; }

        /// <summary>Gets or sets a value indicating whether the temporal endpoints for getting records as of a date should be generated.</summary>
        /// <remarks>This is nullable so that it won't be rendered for non-temporal tables</remarks>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? TemporalGetAsOf { get; set; }
    }
}

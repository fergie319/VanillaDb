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
    }
}

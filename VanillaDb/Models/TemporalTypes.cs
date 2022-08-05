namespace VanillaDb.Models
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

using System.Collections.Generic;

namespace VanillaDb.Models
{
    /// <summary>Contains details about an index such as the fields covered.</summary>
    public class IndexModel
    {
        /// <summary>Gets or sets the fields covered by the index.</summary>
        public IList<FieldModel> Fields { get; set; }

        /// <summary>Gets or sets whether the index is unique (only one record per-value - no duplicates).</summary>
        public bool IsUnique { get; set; }
    }
}

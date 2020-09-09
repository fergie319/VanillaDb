using System.Collections.Generic;

namespace VanillaDb.Models
{
    /// <summary>Contains details about a table - name and fields</summary>
    public class TableModel
    {
        /// <summary>Gets or sets the table name.</summary>
        public string TableName { get; set; }

        /// <summary>Gets or sets the fields.</summary>
        public IList<FieldModel> Fields { get; set; }
    }
}

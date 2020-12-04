using System.Collections.Generic;
using System.Linq;

namespace VanillaDb.Models
{
    /// <summary>Contains details about a table - name and fields</summary>
    public class TableModel
    {
        /// <summary>Gets or sets the table name.</summary>
        public string TableName { get; set; }

        /// <summary>Gets or sets the fields.</summary>
        public IList<FieldModel> Fields { get; set; }

        /// <summary>Gets the primary key for the table model - throws exception if more than one primary key exists.</summary>
        /// <value>The primary key.</value>
        public FieldModel PrimaryKey
        {
            get { return Fields.Single(f => f.IsPrimaryKey); }
        }

        /// <summary>Gets the name of the generated data model for this table.</summary>
        /// <returns></returns>
        public string GetDataModelName()
        {
            return $"{TableName}DataModel";
        }

        /// <summary>Generates the name of the insert stored proc.</summary>
        /// <returns>Insert Stored proc name</returns>
        public string GetInsertProcName()
        {
            return $"USP_{TableName}_Insert";
        }

        /// <summary>Generates the name of the update stored proc.</summary>
        /// <returns>Update Stored proc name</returns>
        public string GetUpdateProcName()
        {
            return $"USP_{TableName}_Update";
        }

        /// <summary>Gets the name of the delete stored procedure.</summary>
        /// <returns>Stored procedure name</returns>
        public string GetDeleteProcName()
        {
            return $"USP_{TableName}_Delete";
        }

        /// <summary>Gets the name of the delete stored procedure.</summary>
        /// <returns>Stored procedure name</returns>
        public string GetDeleteBulkProcName()
        {
            return $"USP_{TableName}_DeleteBulk";
        }
    }
}

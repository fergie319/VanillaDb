using System;

namespace VanillaDb.Models
{
    /// <summary>Contains details about a field's type - include the SQL markup as well as C# type and max length</summary>
    public class FieldTypeModel
    {
        private bool isSqlParameter = true;

        /// <summary>Gets or sets the SQL type for the field.</summary>
        public string SqlType { get; set; }

        /// <summary>Gets or sets the C# type for the field.</summary>
        public Type FieldType { get; set; }

        /// <summary>Gets or sets the maximum length of the field (only applicable to char fields).</summary>
        public int MaxLength { get; set; }

        /// <summary>Gets or sets whether this field is nullable.</summary>
        public bool IsNullable { get; set; }

        /// <summary>Gets or sets a value indicating whether this field is SQL parameter for a stored procedure or not (default is true).</summary>
        public bool IsSqlParameter
        {
            get { return isSqlParameter; }
            set { isSqlParameter = value; }
        }
    }
}

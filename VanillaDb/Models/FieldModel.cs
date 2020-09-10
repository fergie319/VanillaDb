using System;

namespace VanillaDb.Models
{
    /// <summary>Contains details about a field (name, type, whether it is indexed)</summary>
    public class FieldModel
    {
        /// <summary>Gets or sets the name of the field.</summary>
        public string FieldName { get; set; }

        /// <summary>Gets or sets the C# type for the field.</summary>
        public Type FieldType { get; set; }

        /// <summary>Gets or sets the maximum length of the field (only applicable to char fields).</summary>
        public int MaxLength { get; set; }

        /// <summary>Gets or sets the SQL type for the field.</summary>
        public string SqlType { get; set; }

        /// <summary>Gets or sets whether this field is nullable.</summary>
        public bool IsNullable { get; set; }

        /// <summary>Gets or sets whether this field is the table's primary key</summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>Gets or sets whether this field is an identity field.</summary>
        public bool IsIdentity { get; set; }
    }
}

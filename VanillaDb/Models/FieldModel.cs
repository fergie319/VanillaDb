namespace VanillaDb.Models
{
    /// <summary>Contains details about a field (name, type, whether it is indexed)</summary>
    public class FieldModel
    {
        /// <summary>Gets or sets the name of the field.</summary>
        public string FieldName { get; set; }

        /// <summary>Gets or sets the field type details.</summary>
        public FieldTypeModel FieldType { get; set; }

        /// <summary>Gets or sets whether this field is nullable.</summary>
        public bool IsNullable { get; set; }

        /// <summary>Gets or sets whether this field is the table's primary key</summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>Gets or sets whether this field is an identity field.</summary>
        public bool IsIdentity { get; set; }
    }
}

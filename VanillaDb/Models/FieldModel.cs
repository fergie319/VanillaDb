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

        /// <summary>Gets this field's name as a stored procedure parameter.</summary>
        /// <returns>This field as a SQL Stored Procedure parameter name.</returns>
        public string GetParamName()
        {
            return $"@{FieldName.ToCamelCase()}";
        }

        /// <summary>Gets this field's name as a method parameter in C#.</summary>
        /// <returns>this field as a method parameter in C#.</returns>
        public string GetCodeParamName()
        {
            return $"{FieldName.ToCamelCase()}";
        }
    }
}

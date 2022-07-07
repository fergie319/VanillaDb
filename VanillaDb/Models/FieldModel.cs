using System.Collections.Generic;

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

        /// <summary>Gets or sets whether this field is a temporal date field.</summary>
        public bool IsTemporalField { get; set; }

        /// <summary>Gets or sets whether this field is the temporal start or ValidFrom field.</summary>
        public bool IsTemporalStart { get; set; }

        /// <summary>Gets or sets whether this field is the temporal end or ValidTo field.</summary>
        public bool IsTemporalEnd { get; set; }

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

        /// <summary>
        /// Gets a value indicating whether this instance is range field
        /// (semantically supports the =, &gt;, &lt; operators).
        /// </summary>
        public bool IsRangeField
        {
            get
            {
                var isRangeField = true;
                if (FieldType.FieldType == typeof(string) ||
                    FieldType.FieldType == typeof(bool))
                {
                    isRangeField = false;
                }

                return isRangeField;
            }
        }

        /// <summary>Gets the name of the operator.</summary>
        public FieldModel OperatorField
        {
            get
            {
                var operatorField = new FieldModel()
                {
                    FieldName = $"{this.FieldName}_Operator",
                    FieldType = new FieldTypeModel()
                    {
                        FieldType = typeof(QueryOperator),
                        IsNullable = false,
                        SqlType = "INT"
                    },
                };
                return operatorField;
            }
        }

        /// <summary>Gets the parameters for this field - if it is a range field then this will include an operator parameter.</summary>
        /// <returns>Enumerable of FieldModels</returns>
        public IEnumerable<FieldModel> GetParameters()
        {
            yield return this;

            if (IsRangeField)
            {
                var operatorField = OperatorField;
                yield return operatorField;
            }
        }

        /// <summary>Gets the method parameter declaration for this field.</summary>
        public string GetMethodParamDeclaration()
        {
            var result = $"{FieldType.GetAliasOrName()} {FieldName.ToCamelCase()}";
            if (FieldType.FieldType == typeof(QueryOperator))
            {
                result += " = QueryOperator.Equals";
            }

            return result;
        }

        /// <summary>Gets the expression for this field in a where clause.</summary>
        /// <returns></returns>
        public string GetWhereExpression()
        {
            var @operatorParam = OperatorField.GetParamName();
            var @fieldParam = this.GetParamName();
            string whereExpression;
            if (IsRangeField)
            {
                // GREATER-0/LESS-1/EQUAL-2
                // Note: GreaterThan means we want all values Greater Than the field parameter,
                //       so they appear reversed in the implementation.
                whereExpression =
                    $"(({@operatorParam} = {(int)QueryOperator.Equals} AND {fieldParam} = {FieldName}) OR" +
                    $" ({@operatorParam} = {(int)QueryOperator.GreaterThan} AND {fieldParam} <= {FieldName}) OR" +
                    $" ({@operatorParam} = {(int)QueryOperator.LessThan} AND {fieldParam} >= {FieldName}))";
            }
            else
            {
                // Basic Equals check
                whereExpression = $"{FieldName} = @{FieldName.ToCamelCase()}";
            }

            return whereExpression;
        }

        /// <summary>Gets the line of code for adding the parameter to the sql command.</summary>
        /// <returns>code string</returns>
        public string GetAddParamCode()
        {
            var paramName = this.GetParamName();
            var codeName = this.GetCodeParamName();
            var castType = (this.FieldType.FieldType == typeof(QueryOperator))
                            ? "(int)"
                            : string.Empty;
            var addParamCode = $"command.Parameters.AddWithValue(\"{paramName}\", {castType}{codeName});";
            return addParamCode;
        }
    }
}

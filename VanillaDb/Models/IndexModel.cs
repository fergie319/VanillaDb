using System;
using System.Collections.Generic;
using System.Linq;

namespace VanillaDb.Models
{
    /// <summary>Contains details about an index such as the fields covered.</summary>
    public class IndexModel
    {
        /// <summary>Gets or sets the table this index applies to.</summary>
        public TableModel Table { get; set; }

        /// <summary>Gets or sets the fields covered by the index.</summary>
        public IList<FieldModel> Fields { get; set; }

        /// <summary>Gets or sets whether the index is unique (only one record per-value - no duplicates).</summary>
        public bool IsUnique { get; set; }

        /// <summary>Gets or sets whether this index is for the table's primary key.</summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>Gets or sets the parameters for the fields</summary>
        /// <remarks>
        /// This list is different because we might have additional parameters for specifying the operator to use.
        /// </remarks>
        public IEnumerable<FieldModel> Parameters
        {
            get
            {
                var parameters = new List<FieldModel>();
                foreach (var field in Fields)
                {
                    foreach (var parameter in field.GetParameters())
                    {
                        parameters.Add(parameter);
                    }
                }

                parameters.Sort((p1, p2) =>
                {
                    var result = 0;
                    var p1Type = p1.FieldType.FieldType;
                    var p2Type = p2.FieldType.FieldType;
                    if (p1Type != p2Type)
                    {
                        if (p1Type == typeof(QueryOperator))
                        {
                            result = 1;
                        }
                        else if (p2Type == typeof(QueryOperator))
                        {
                            result = -1;
                        }
                    }

                    return result;
                });

                return parameters;
            }
        }

        /// <summary>Creates a readable list of the indexes' field names (joined by 'and').</summary>
        /// <returns>Human readable list of fields.</returns>
        public string ReadableFields()
        {
            return string.Join(" and ", Fields.Select(f => f.FieldName));
        }

        /// <summary>Generates the get by index parameters XML comments.</summary>
        /// <returns></returns>
        public string GetByIndexParamsXmlComments()
        {
            var indent = "        ";
            var xmlParams = Parameters.Select(f => $"/// <param name=\"{f.FieldName.ToCamelCase()}\">The {f.FieldName} value.</param>");
            return string.Join($"{Environment.NewLine}{indent}", xmlParams);
        }

        /// <summary>Gets the return type of a GetByIndex method based on this index.</summary>
        /// <returns></returns>
        public string GetByIndexReturnType()
        {
            var returnType = (IsUnique) ? Table.GetDataModelName()
                                        : $"IEnumerable<{Table.GetDataModelName()}>";
            return returnType;
        }

        /// <summary>Gets the name of the GetByIndex method.</summary>
        /// <returns></returns>
        public string GetByIndexMethodName()
        {
            return $"GetBy{string.Join("And", Fields.Select(f => f.FieldName))}";
        }

        /// <summary>Generates the get by index method parameters.</summary>
        /// <returns></returns>
        public string GetByIndexMethodParams()
        {
            var fields = Parameters.Select(f => f.GetMethodParamDeclaration());
            return string.Join(", ", fields);
        }

        /// <summary>Generates the bulk get by index parameters XML comments.</summary>
        /// <returns></returns>
        public string BulkGetByIndexParamsXmlComments()
        {
            var indent = "        ";
            var xmlParams = Fields.Select(f => $"/// <param name=\"{f.FieldName.ToCamelCase()}s\">The {f.FieldName.ToCamelCase()} values.</param>");
            return string.Join($"{Environment.NewLine}{indent}", xmlParams);
        }

        /// <summary>Generates the bulk get by index method parameters.</summary>
        /// <returns></returns>
        public string BulkGetByIndexMethodParams()
        {
            return string.Join(", ", Fields.Select(f => $"IEnumerable<{f.FieldType.GetAliasOrName()}> {f.FieldName.ToCamelCase()}s"));
        }

        /// <summary>Generates the bulk type identifier table.</summary>
        /// <returns>Type ID Table Name.</returns>
        public string GetBulkTypeIdTable()
        {
            var fieldNames = Fields.Select(f => f.FieldName);
            return $"Type_{string.Join("_", fieldNames)}_Table";
        }

    }
}

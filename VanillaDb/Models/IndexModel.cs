﻿using System;
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
        /// <param name="temporalType">The Temporal Type of parameters to get - Default is the default :-)</param>
        /// <param name="excludeQueryOperators">if set to <c>true</c> [exclude query operators].</param>
        /// <returns>List of parameters for the stored proc and dataprovider interface</returns>
        /// <remarks>This list is different because we might have additional parameters for specifying the operator to use.</remarks>
        public IEnumerable<FieldModel> Parameters(TemporalTypes temporalType = TemporalTypes.Default, bool excludeQueryOperators = false)
        {
            var parameters = new List<FieldModel>();
            foreach (var field in Fields)
            {
                if (!excludeQueryOperators)
                {
                    foreach (var parameter in field.GetParameters())
                    {
                        parameters.Add(parameter);
                    }
                }
                else
                {
                    parameters.Add(field);
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

            // Add the temporal enum option if supported
            if (temporalType != TemporalTypes.Default)
            {
                parameters.Add(new FieldModel()
                {
                    FieldName = "TemporalOperator",
                    FieldType = new FieldTypeModel()
                    {
                        FieldType = typeof(TemporalTypes),
                        IsSqlParameter = false,
                    },
                    IsArtificialField = true
                });

                if (temporalType == TemporalTypes.AsOf)
                {
                    parameters.Add(new FieldModel()
                    {
                        FieldName = "AsOfDate",
                        FieldType = new FieldTypeModel()
                        {
                            FieldType = typeof(DateTime),
                            IsNullable = true,
                            SqlType = "DATETIME2"
                        },
                        IsArtificialField = true
                    });
                }
            }

            return parameters;
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
            var temporalType = TemporalTypes.Default;
            if (Table.IsTemporal && Table.Config.TemporalGetAsOf.Value)
            {
                temporalType = TemporalTypes.AsOf;
            }
            else if (Table.IsTemporal && Table.Config.TemporalGetAll.Value)
            {
                temporalType = TemporalTypes.All;
            }

            var xmlParams = Parameters(temporalType).Select(f => $"/// <param name=\"{f.FieldName.ToCamelCase()}\">The {f.FieldName} value.</param>");
            return string.Join($"{Environment.NewLine}{indent}", xmlParams);
        }

        /// <summary>Gets the return type of a GetByIndex method based on this index.</summary>
        /// <returns></returns>
        public string GetByIndexReturnType()
        {
            var isSingleResult = IsUnique && (!Table.IsTemporal || !Table.Config.TemporalGetAll.Value);
            var returnType = (isSingleResult) ? Table.GetDataModelName()
                                              : $"IEnumerable<{Table.GetDataModelName()}>";
            return returnType;
        }

        /// <summary>Gets the name of the GetByIndex method.</summary>
        /// <returns></returns>
        public string GetByIndexMethodName()
        {
            return $"GetBy{string.Join("And", Fields.Select(f => f.FieldName))}";
        }

        /// <summary>Gets the name of the GetByIndex stored procedure.</summary>
        /// <param name="temporalType">The temporal type of procedure.</param>
        /// <returns>GetByIndex Proc Name</returns>
        public string GetByIndexProcName(TemporalTypes temporalType)
        {
            var procNameFields = Fields.Select(f => f.FieldName);
            var procName = $"USP_{Table.TableName}_GetBy{string.Join("_", procNameFields)}";
            if (Table.IsTemporal && temporalType == TemporalTypes.AsOf)
            {
                procName += "_AsOf";
            }
            else if (temporalType == TemporalTypes.All)
            {
                procName += "_All";
            }

            return procName;
        }

        /// <summary>Generates the get by index method parameters.</summary>
        /// <returns></returns>
        public string GetByIndexMethodParams()
        {
            var temporalType = TemporalTypes.Default;
            if (Table.IsTemporal && Table.Config.TemporalGetAsOf.Value)
            {
                temporalType = TemporalTypes.AsOf;
            }
            else if (Table.IsTemporal && Table.Config.TemporalGetAll.Value)
            {
                temporalType = TemporalTypes.All;
            }

            var fields = Parameters(temporalType).Select(f => f.GetMethodParamDeclaration());
            return string.Join(", ", fields);
        }

        /// <summary>Generates the bulk get by index parameters XML comments.</summary>
        /// <returns></returns>
        public string BulkGetByIndexParamsXmlComments()
        {
            var temporalType = TemporalTypes.Default;
            if (Table.IsTemporal && Table.Config.TemporalGetAsOf.Value)
            {
                temporalType = TemporalTypes.AsOf;
            }
            else if (Table.IsTemporal && Table.Config.TemporalGetAll.Value)
            {
                temporalType = TemporalTypes.All;
            }

            var indent = "        ";
            var xmlParams = Parameters(temporalType, true).Select(f =>
                (f.IsArtificialField)
                    ? $"/// <param name=\"{f.FieldName.ToCamelCase()}\">The {f.FieldName.ToCamelCase()}</param>"
                    : $"/// <param name=\"{f.FieldName.ToCamelCase()}s\">The {f.FieldName.ToCamelCase()} values.</param>");
            return string.Join($"{Environment.NewLine}{indent}", xmlParams);
        }

        /// <summary>Generates the bulk get by index method parameters.</summary>
        /// <returns></returns>
        public string BulkGetByIndexMethodParams()
        {
            var temporalType = TemporalTypes.Default;
            if (Table.IsTemporal && Table.Config.TemporalGetAsOf.Value)
            {
                temporalType = TemporalTypes.AsOf;
            }
            else if (Table.IsTemporal && Table.Config.TemporalGetAll.Value)
            {
                temporalType = TemporalTypes.All;
            }

            return string.Join(", ", Parameters(temporalType, true).Select(f =>
                (f.IsArtificialField)
                    ? $"{f.FieldType.GetAliasOrName()} {f.FieldName.ToCamelCase()}"
                    : $"IEnumerable<{f.FieldType.GetAliasOrName()}> {f.FieldName.ToCamelCase()}s"));
        }

        /// <summary>Gets the name of the GetbyIndex bulk stored procedure.</summary>
        /// <param name="temporalType">The temporal type of procedure.</param>
        /// <returns>GetByIndex Bulk Proc Name</returns>
        public string BulkGetByIndexProcName(TemporalTypes temporalType)
        {
            var procNameFields = Fields.Select(f => f.FieldName);
            var procName = $"USP_{Table.TableName}_GetBy{string.Join("_", procNameFields)}_Bulk";
            if (Table.IsTemporal && temporalType == TemporalTypes.AsOf)
            {
                procName += "_AsOf";
            }
            else if (temporalType == TemporalTypes.All)
            {
                procName += "_All";
            }

            return procName;
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

using System;
using System.Collections.Generic;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.GetProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class GetAllStoredProc : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private TemporalTypes TemporalType { get; set; }

        /// <summary>Initializes a new instance of the <see cref="GetAllStoredProc" /> class.</summary>
        /// <param name="table">The table to query.</param>
        /// <param name="temporalType">The type of temporal query to generate - only for temporal tables.</param>
        public GetAllStoredProc(TableModel table, TemporalTypes temporalType = TemporalTypes.Default)
        {
            Table = table;
            TemporalType = temporalType;
            if (TemporalType != TemporalTypes.Default && !Table.IsTemporal)
            {
                throw new InvalidOperationException("Cannot generate temporal procedures for a non-temporal table.");
            }
        }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "sql";

        /// <summary>Generates the portion of the procedure name that is composed of the indexed fields.</summary>
        /// <returns>Underscore-separated field names</returns>
        public string GenerateName()
        {
            var procName = $"USP_{Table.TableName}_GetAll";
            if (Table.IsTemporal && TemporalType == TemporalTypes.AsOf)
            {
                procName += "_AsOf";
            }
            else if (TemporalType == TemporalTypes.All)
            {
                procName += "_All";
            }

            return procName;
        }

        /// <summary>Generates the stored procedure's parameter list.</summary>
        /// <returns>Newline separated stored procedure parameters</returns>
        public string GenerateProcParameters()
        {
            // Add the temporal enum option if supported
            var parameters = new List<FieldModel>();
            if (TemporalType != TemporalTypes.Default)
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

                if (TemporalType == TemporalTypes.AsOf)
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

            var procParams = parameters
                .Where(f => f.FieldType.IsSqlParameter)
                .Select(f => $"    @{f.FieldName.ToCamelCase()} {f.FieldType.SqlType}");
            return string.Join("," + Environment.NewLine, procParams);
        }
    }
}

using System;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.GetProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class GetBySingleStoredProc
    {
        private TableModel Table { get; set; }

        private IndexModel Index { get; set; }

        /// <summary>Initializes a new instance of the <see cref="GetBySingleStoredProc" /> class.</summary>
        /// <param name="table">The table to query.</param>
        /// <param name="index">The index to query by.</param>
        public GetBySingleStoredProc(TableModel table, IndexModel index)
        {
            Table = table;
            Index = index;
        }

        /// <summary>Generates the portion of the procedure name that is composed of the indexed fields.</summary>
        /// <returns>Underscore-separated field names</returns>
        public string GenerateName()
        {
            var procNameFields = Index.Fields.Select(f => f.FieldName);
            return $"USP_{Table.TableName}_GetBy{string.Join("_", procNameFields)}";
        }

        /// <summary>Generates the stored procedure's parameter list.</summary>
        /// <returns>Newline separated stored procedure parameters</returns>
        public string GenerateProcParameters()
        {
            var procParams = Index.Fields
                .Select(f => $"    @{f.FieldName.ToCamelCase()} {f.FieldType.SqlType}");
            return string.Join("," + Environment.NewLine, procParams);
        }

        /// <summary>Generates the fields for the SELECT clause.</summary>
        /// <returns>Comma-separated Table Fields</returns>
        public string GenerateSelectFields()
        {
            var selectFields = Table.Fields.Select(f => f.FieldName);
            return string.Join(", ", selectFields);
        }

        /// <summary>Generates the where clause for the query.</summary>
        /// <returns>Where clause</returns>
        public string GenerateWhereClause()
        {
            var indexFields = Index.Fields.Select(f => $"{f.FieldName} = @{f.FieldName.ToCamelCase()}");
            return string.Join(" AND ", indexFields);
        }
    }
}

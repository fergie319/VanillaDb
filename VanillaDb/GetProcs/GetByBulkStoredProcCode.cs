using System;
using System.Linq;
using VanillaDb.Models;
using VanillaDb.TypeTables;

namespace VanillaDb.GetProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.GetProcs.GetByBulkStoredProcBase" />
    public partial class GetByBulkStoredProc : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IndexModel Index { get; set; }

        private TemporalTypes TemporalType { get; set; }

        /// <summary>Initializes a new instance of the <see cref="GetBySingleStoredProc" /> class.</summary>
        /// <param name="table">The table to query.</param>
        /// <param name="index">The index to query by.</param>
        /// <param name="temporalType">The type of temporal query to generate - only for temporal tables.</param>
        public GetByBulkStoredProc(TableModel table, IndexModel index, TemporalTypes temporalType = TemporalTypes.Default)
        {
            Table = table;
            Index = index;
            TemporalType = temporalType;
            if (TemporalType != TemporalTypes.Default && !Table.IsTemporal)
            {
                throw new InvalidOperationException("Cannot generate temporal procedures for a non-temporal table.");
            }
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Procedure name and file name (without .sql).</returns>
        public string GenerateName()
        {
            var procNameFields = Index.Fields.Select(f => f.FieldName);
            var procName = $"USP_{Table.TableName}_GetBy{string.Join("_", procNameFields)}_Bulk";
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

        /// <summary>Generates the GetBy(Bulk) stored procedure's parameter name.</summary>
        /// <returns>Newline separated stored procedure parameters</returns>
        public string GenerateBulkProcParameter(IndexModel index)
        {
            var typeTable = new TypeTable(index);
            var fieldNames = index.Fields.Select(f => f.FieldName.ToCamelCase());
            return $"@{string.Join("_", fieldNames)}s";
        }

        /// <summary>Generates the stored procedure's parameter list.</summary>
        /// <returns>Newline separated stored procedure parameters</returns>
        public string GenerateProcParameters()
        {
            var typeTable = new TypeTable(Index);
            var fieldNames = Index.Fields.Select(f => f.FieldName.ToCamelCase());
            return $"    {GenerateBulkProcParameter(Index)} {Table.Schema}.{typeTable.GenerateName()} READONLY";
        }

        /// <summary>Generates the fields for the SELECT clause.</summary>
        /// <returns>Comma-separated Table Fields</returns>
        public string GenerateSelectFields()
        {
            var alias = GetTableAlias();
            var selectFields = Table.Fields.Select(f => $"{alias}.{f.FieldName}");
            return string.Join(", ", selectFields);
        }

        /// <summary>Gets the table alias to use in the query.</summary>
        /// <returns>single character table alias.</returns>
        public string GetTableAlias()
        {
            return "A";
        }

        /// <summary>Generates the where clause for the select statement.</summary>
        /// <returns>SQL Where Clause.</returns>
        public string GenerateJoinClause()
        {
            var alias = GetTableAlias();
            var param = GenerateBulkProcParameter(Index);
            var paramAlias = "B";
            var fieldClauses = Index.Fields.Select(f => $"{alias}.{f.FieldName} = {paramAlias}.{f.FieldName}");
            var andClauses = string.Join(" AND " + Environment.NewLine, fieldClauses);
            return $"{param} AS {paramAlias} ON ({andClauses})";
        }
    }
}

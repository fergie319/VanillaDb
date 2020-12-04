using System;
using System.Linq;
using VanillaDb.Models;
using VanillaDb.TypeTables;

namespace VanillaDb.DeleteProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.GetProcs.GetByBulkStoredProcBase" />
    public partial class DeleteBulkStoredProc
    {
        private TableModel Table { get; set; }

        private IndexModel Index { get; set; }

        /// <summary>Initializes a new instance of the <see cref="DeleteBulkStoredProc" /> class.</summary>
        /// <param name="table">The table to query.</param>
        /// <param name="index">The index to query by.</param>
        public DeleteBulkStoredProc(TableModel table, IndexModel index)
        {
            Table = table;
            Index = index;
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Procedure name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return Table.GetDeleteBulkProcName();
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
            return $"    {GenerateBulkProcParameter(Index)} dbo.{typeTable.GenerateName()} READONLY";
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

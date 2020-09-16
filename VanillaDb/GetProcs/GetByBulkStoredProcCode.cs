using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using System;
using System.Linq;
using VanillaDb.Models;
using VanillaDb.TypeTables;

namespace VanillaDb.GetProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.GetProcs.GetByBulkStoredProcBase" />
    public partial class GetByBulkStoredProc
    {
        private TableModel Table { get; set; }

        private IndexModel Index { get; set; }

        /// <summary>Initializes a new instance of the <see cref="GetBySingleStoredProc" /> class.</summary>
        /// <param name="table">The table to query.</param>
        /// <param name="index">The index to query by.</param>
        public GetByBulkStoredProc(TableModel table, IndexModel index)
        {
            Table = table;
            Index = index;
        }
        // TODO: Implement the transform.
        // TODO: Add method to all transform classes for getting proc/file name.

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Procedure name and file name (without .sql).</returns>
        public string GenerateName()
        {
            var fieldNames = Index.Fields.Select(f => f.FieldName);
            return $"USP_{Table.TableName}_GetBy{string.Join("_", fieldNames)}_Bulk";
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

        /// <summary>Generates the fields for the SELECT clause.</summary>
        /// <returns>Comma-separated Table Fields</returns>
        public string GenerateSelectFields()
        {
            var selectFields = Table.Fields.Select(f => f.FieldName);
            return string.Join(", ", selectFields);
        }

        /// <summary>Gets the table alias to use in the query.</summary>
        /// <returns>single character table alias.</returns>
        public string GetTableAlias()
        {
            return Table.TableName[0].ToString();
        }

        /// <summary>Generates the where clause for the select statement.</summary>
        /// <returns>SQL Where Clause.</returns>
        public string GenerateJoinClause()
        {
            var alias = GetTableAlias();
            var param = GenerateBulkProcParameter(Index);
            var fieldClauses = Index.Fields.Select(f => $"{alias}.{f.FieldName} = {param}.{f.FieldName})");
            var andClauses = string.Join(" AND " + Environment.NewLine, fieldClauses);
            return $"{param} ON ({andClauses})";
        }
    }
}

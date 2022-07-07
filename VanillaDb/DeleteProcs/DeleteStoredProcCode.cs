using System;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.DeleteProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class DeleteStoredProc : ICodeTemplate
    {
        private TableModel Table { get; set; }

        /// <summary>Initializes a new instance of the <see cref="DeleteStoredProc"/> class.</summary>
        /// <param name="table">The table.</param>
        public DeleteStoredProc(TableModel table)
        {
            Table = table;
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Stored Procedure name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return Table.GetDeleteProcName();
        }

        /// <summary>Generates the stored procedure's parameter list.</summary>
        /// <returns></returns>
        public string GenerateProcParameters()
        {
            var insertParams = Table.Fields
                .Where(f => f.IsPrimaryKey)
                .Select(f => $"    {f.GetParamName()} {f.FieldType.SqlType}");
            return string.Join("," + Environment.NewLine, insertParams);
        }

        /// <summary>Generates the where clause for the Delete sql statement.</summary>
        /// <returns>Where clause.</returns>
        public string GenerateWhereClause()
        {
            var deleteParams = Table.Fields
                .Where(f => f.IsPrimaryKey)
                .Select(f => $"{f.FieldName} = {f.GetParamName()}");
            return string.Join(" AND ", deleteParams);
        }
    }
}

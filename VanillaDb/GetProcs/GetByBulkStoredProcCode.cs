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
        /// <returns>Type Table name and file name (without .sql).</returns>
        public string GenerateName()
        {
            var fieldNames = Index.Fields.Select(f => f.FieldName);
            return $"USP_{Table.TableName}_GetBy{string.Join("_", fieldNames)}_BULK";
        }
    }
}

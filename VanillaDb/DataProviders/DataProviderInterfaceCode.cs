using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaDb.Models;

namespace VanillaDb.DataProviders
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.DataProviders.DataProviderInterfaceBase" />
    public partial class DataProviderInterface
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Initializes a new instance of the <see cref="DataProviderInterface" /> class.</summary>
        /// <param name="table">The table model.</param>
        /// <param name="indexes">The indexes.</param>
        public DataProviderInterface(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Class name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return $"I{Table.TableName}DataProvider";
        }
    }
}

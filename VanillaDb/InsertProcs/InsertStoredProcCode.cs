using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaDb.Models;

namespace VanillaDb.InsertProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class InsertStoredProc
    {
        private TableModel Table { get; set; }

        /// <summary>Initializes a new instance of the <see cref="InsertStoredProc"/> class.</summary>
        /// <param name="table">The table.</param>
        public InsertStoredProc(TableModel table)
        {
            Table = table;
        }
    }
}

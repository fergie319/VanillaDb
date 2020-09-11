using VanillaDb.Models;

namespace VanillaDb.TypeTables
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class TypeTable
    {
        private IndexModel Index { get; set; }

        /// <summary>Initializes a new instance of the <see cref="TypeTable"/> class.</summary>
        /// <param name="index">The index for which to create the type table.</param>
        public TypeTable(IndexModel index)
        {
            Index = index;
        }
    }
}

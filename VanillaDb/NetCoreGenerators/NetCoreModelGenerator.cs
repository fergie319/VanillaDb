using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.NetCoreGenerators
{
    /// <summary>Class for generating .NET Core Controllers and Models.</summary>
    public class NetCoreModelGenerator : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "cs";

        /// <summary>Initializes a new instance of the <see cref="NetCoreModelGenerator"/> class.</summary>
        /// <param name="table">The table definition.</param>
        /// <param name="indexes">The indexes on the table.</param>
        public NetCoreModelGenerator(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Generates a .NET Core Model that works with the data provider data model.</summary>
        /// <returns>result string.</returns>
        public string TransformText()
        {
            return string.Empty;
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}Model";
        }
    }
}

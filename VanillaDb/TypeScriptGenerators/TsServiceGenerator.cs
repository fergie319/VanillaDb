using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.TypeScriptGenerators
{
    /// <summary>Class for generating a TypeScript Axios service.</summary>
    public class TsServiceGenerator : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "ts";


        /// <summary>Initializes a new instance of the <see cref="TsServiceGenerator"/> class.</summary>
        /// <param name="table">The table definition.</param>
        /// <param name="indexes">The indexes on the table.</param>
        public TsServiceGenerator(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Generates a TypeScript Axios service that works with the endpoints defined in the .NET Core Controller.</summary>
        /// <returns>result string.</returns>
        public string TransformText()
        {
            return string.Empty;
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}Service";
        }
    }
}

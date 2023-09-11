using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.TypeScriptGenerators
{
    /// <summary>Class for generating a TypeScript Model.</summary>
    public class TsModelGenerator : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Gets the name to use for variables that reference the table name.</summary>
        private string TableVariableName
        {
            get { return Table.TableAlias.ToCamelCase(); }
        }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "ts";

        /// <summary>Initializes a new instance of the <see cref="TsModelGenerator"/> class.</summary>
        /// <param name="table">The table definition.</param>
        /// <param name="indexes">The indexes on the table.</param>
        public TsModelGenerator(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Generates a TypeScript Model that corresponds to the .NET Core Model.</summary>
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

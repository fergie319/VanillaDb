using System;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.DataProviders
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.DataProviders.DataModelBase" />
    public partial class DataModel : ICodeTemplate
    {
        private TableModel Table { get; set; }

        /// <summary>Initializes a new instance of the <see cref="DataModel" /> class.</summary>
        /// <param name="table">The table model.</param>
        public DataModel(TableModel table)
        {
            Table = table;
        }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "cs";

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Class name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}DataModel";
        }

        /// <summary>Generates the properties for the datamodel.</summary>
        /// <returns>C# getter/setter properties.</returns>
        public string GenerateProperties()
        {
            var indent = "        ";
            var properties = Table.Fields
                .Select(f => $"/// <summary>Gets or sets the {f.FieldName}.</summary>{Environment.NewLine}" +
                             $"{indent}public {f.FieldType.GetAliasOrName()} {f.FieldName} {{ get; set; }}");
            return string.Join($"{Environment.NewLine}{Environment.NewLine}{indent}", properties);
        }
    }
}

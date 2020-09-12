using System;
using System.Linq;
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

        /// <summary>Gets the name of the file.</summary>
        /// <returns>Type Table name and file name (without .sql).</returns>
        public string GenerateFileName()
        {
            var fieldNames = Index.Fields.Select(f => f.FieldName);
            return $"Type_{string.Join("_", fieldNames)}_Table";
        }

        /// <summary>Generates the fields for the type table.</summary>
        /// <returns>List of table field definitions.</returns>
        public string GenerateFields()
        {
            var fieldDefs = Index.Fields.Select(f => $"    {f.FieldName} {f.FieldType.SqlType}");
            return string.Join(Environment.NewLine, fieldDefs);
        }
    }
}

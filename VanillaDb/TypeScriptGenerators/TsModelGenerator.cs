using System;
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
            var result = $@"
export default interface I{Table.TableAlias}Model
{{
{GenerateProperties()}}}
";

            return result;
        }

        private string GenerateProperties()
        {
            var propertyStatements = new List<string>();
            foreach (var field in Table.Fields)
            {
                var nullableChar = (field.IsNullable) ? "?" : string.Empty;
                propertyStatements.Add($"/** Gets or sets the {field.FieldName}. */");
                propertyStatements.Add($"{field.FieldName.ToCamelCase()}{nullableChar}: {GetFieldTsType(field.FieldType)}{Environment.NewLine}");
            }

            return "    " + string.Join($"{Environment.NewLine}    ", propertyStatements);
        }

        private string GetFieldTsType(FieldTypeModel fieldType)
        {
            switch (fieldType.GetAliasOrName().Replace("?", string.Empty))
            {
                case "int":
                case "short":
                case "byte":
                case "long":
                case "double":
                case "decimal":
                    return "number";
                case "string":
                    return "string";
                case "bool":
                    return "boolean";
                case "DateTime":
                    return "Date";
                default:
                    return fieldType.GetAliasOrName();
            }
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"I{Table.TableAlias}Model";
        }
    }
}

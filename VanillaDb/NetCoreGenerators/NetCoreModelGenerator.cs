using System;
using System.Collections.Generic;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.NetCoreGenerators
{
    /// <summary>Class for generating .NET Core Controllers and Models.</summary>
    public class NetCoreModelGenerator : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Gets the name to use for variables that reference the table name.</summary>
        private string TableVariableName
        {
            get { return Table.TableAlias.ToCamelCase(); }
        }

        /// <summary>Gets the table alias.</summary>
        private string TableAlias { get { return Table.TableAlias; } }

        private FieldModel IdField { get { return Table.PrimaryKey; } }

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
            return
                BeginNamespace() +
                NewModelClass() +
                ModelClass() +
                EndNamespace();
        }

        private string BeginNamespace()
        {
            return $@"using {Table.Namespace}.DataProviders.{TableAlias};

namespace {Table.Namespace}.Models
{{";
        }

        private string EndNamespace()
        {
            return @"}
";
        }

        private string NewModelClass()
        {
            return $@"
    /// <summary>Service Model for New {TableAlias}s</summary>
    public class New{TableAlias}Model
    {{
{GenerateProperties(Table.InsertFields)}
    }}
";
        }

        private string ModelClass()
        {
            return $@"
    /// <summary>Service Model for {TableAlias}s - inherits New{TableAlias}Model and defines auto-populated fields.</summary>
    public class {TableAlias}Model : New{TableAlias}Model
    {{
{GenerateConstructors(Table.Fields, Table.InsertFields)}
{GenerateProperties(Table.Fields.Except(Table.InsertFields))}
{GenerateToDataMethod(Table.Fields)}
    }}
";
        }

        private string GenerateConstructors(IEnumerable<FieldModel> fields, IEnumerable<FieldModel> insertFields)
        {
            var newAssignmentStatements = GetAssignmentStatements(insertFields, string.Empty, $"new{TableAlias}.");
            var dataAssignmentStatements = GetAssignmentStatements(fields, string.Empty, $"{TableVariableName}Data.");
            return $@"        /// <summary>Initializes a new instance of the <see cref=""{TableAlias}Model""/> class.</summary>
        public {TableAlias}Model()
        {{
        }}

        /// <summary>Initializes a new instance of the <see cref=""{TableAlias}Model""/> class from a New{TableAlias}Model.</summary>
        /// <param name=""new{TableAlias}"">The new {TableVariableName} instance to initialize with.</param>
        public {TableAlias}Model(New{TableAlias}Model new{TableAlias})
        {{
            {string.Join($"{Environment.NewLine}            ", newAssignmentStatements)}
        }}

        /// <summary>Initializes a new instance of the <see cref=""{TableAlias}Model""/> class from an {TableAlias}DataModel.</summary>
        /// <param name=""{TableVariableName}Data"">The {TableVariableName} data to initialize with.</param>
        public {TableAlias}Model({TableAlias}DataModel {TableVariableName}Data)
        {{
            {string.Join($"{Environment.NewLine}            ", dataAssignmentStatements)}
        }}
";
        }

        private string GenerateProperties(IEnumerable<FieldModel> fields)
        {
            var propertyStatements = new List<string>();
            foreach (var field in fields)
            {
                // The compiler complains if strings are not assigned in the constructor
                // So we perform an inline assignment to prevent the compiler warning
                var assignment = string.Empty;
                if (field.FieldType.FieldType == typeof(string))
                {
                    assignment = " = string.Empty;";
                }

                propertyStatements.Add($"/// <summary>Gets or sets the {field.FieldName}.</summary>");
                propertyStatements.Add($"public {field.FieldType.GetAliasOrName()} {field.FieldName} {{ get; set; }}{assignment}{Environment.NewLine}");
            }

            return "        " + string.Join($"{Environment.NewLine}        ", propertyStatements);
        }

        private string GenerateToDataMethod(IEnumerable<FieldModel> fields)
        {
            var assignmentStatements = GetAssignmentStatements(fields, "result.", string.Empty);
            return $@"        /// <summary>Converts the {TableAlias}Model to {TableAlias}DataModel.</summary>
        /// <returns>{TableAlias}Data instance.</returns>
        public {TableAlias}DataModel ToData()
        {{
            var result = new {TableAlias}DataModel();
            {string.Join($"{Environment.NewLine}            ", assignmentStatements)}
            return result;
        }}";
        }

        private IEnumerable<string> GetAssignmentStatements(IEnumerable<FieldModel> fields, string toVariableName, string fromVariableName)
        {
            var assignmentStatements = new List<string>();
            foreach (var field in fields)
            {
                assignmentStatements.Add($"{toVariableName}{field.FieldName} = {fromVariableName}{field.FieldName};");
            }

            return assignmentStatements;
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}Model";
        }
    }
}

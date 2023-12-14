using System;
using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.NetCoreGenerators
{
    /// <summary>Class for generating .NET Core Controllers and Models.</summary>
    public class NetCoreControllerGenerator : ICodeTemplate
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


        /// <summary>Initializes a new instance of the <see cref="NetCoreControllerGenerator"/> class.</summary>
        /// <param name="table">The table definition.</param>
        /// <param name="indexes">The indexes on the table.</param>
        public NetCoreControllerGenerator(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Generates a .NET Core Controller that works with the dataprovider.</summary>
        /// <returns>result string.</returns>
        public string TransformText()
        {
            var getByIdMethod = (IdField != null) ? GetByIdMethod() + Environment.NewLine : string.Empty;
            var insertMethod = (Table.Config.Insert) ? CreateMethod() + Environment.NewLine : string.Empty;
            var updateMethod = (Table.Config.Update) ? UpdateMethod() + Environment.NewLine : string.Empty;
            var deleteMethod = (Table.Config.Delete) ? DeleteMethod() : string.Empty;
            return
                BeginClass() + Environment.NewLine +
                GetAllMethod() + Environment.NewLine +
                getByIdMethod +
                insertMethod +
                updateMethod +
                deleteMethod +
                EndClass();
        }

        private string BeginClass()
        {
            return $@"using {Table.Namespace}.DataProviders.{TableAlias};
using {Table.Namespace}.Models;
using {Table.Namespace}.Filters;
using Microsoft.AspNetCore.Mvc;

namespace {Table.Namespace}.Controllers
{{
    /// <summary>Exposes endpoints for managing {TableAlias}s.</summary>
    /// <seealso cref=""Microsoft.AspNetCore.Mvc.ControllerBase"" />
    [ApiController]
    [Route(""api/[controller]"")]
    public class {TableAlias}sController : ControllerBase
    {{
        private ILogger<{TableAlias}sController> Logger {{ get; set; }}

        private I{TableAlias}DataProvider {TableAlias}DataProvider {{ get; set; }}

        /// <summary>Initializes a new instance of the <see cref=""{TableAlias}sController"" /> class.</summary>
        /// <param name=""logger"">The logger.</param>
        /// <param name=""{TableVariableName}sDataProvider"">The {TableVariableName}s data provider.</param>
        public {TableAlias}sController(
            ILogger<{TableAlias}sController> logger,
            I{TableAlias}DataProvider {TableVariableName}sDataProvider)
        {{
            Logger = logger;
            {TableAlias}DataProvider = {TableVariableName}sDataProvider;
        }}
";
        }

        private string EndClass()
        {
            return @"    }
}
";
        }

        private string GetAllMethod()
        {
            return $@"        /// <summary>Gets all {TableVariableName}s in the system.</summary>
        /// <returns>List of {TableAlias}s</returns>
        [HttpGet]
        public async Task<IEnumerable<{TableAlias}Model>> Get{TableAlias}s()
        {{
            var {TableVariableName}s = await {TableAlias}DataProvider.GetAll();
            return {TableVariableName}s.Select(d => new {TableAlias}Model(d));
        }}
";
        }

        private string GetByIdMethod()
        {
            return $@"        /// <summary>Gets the {TableVariableName} with the given {IdField.FieldName}.</summary>
        /// <returns>{TableAlias} with the given {IdField.FieldName}.</returns>
        [Route(""{{{IdField.GetCodeParamName()}}}"")]
        [HttpGet]
        public async Task<{TableAlias}Model> Get{TableAlias}(int {IdField.GetCodeParamName()})
        {{
            var {TableVariableName} = await {TableAlias}DataProvider.GetBy{IdField.FieldName}({IdField.GetCodeParamName()});
            if ({TableVariableName} == null)
            {{
                throw new HttpResponseException(404, $""No {TableVariableName} with {IdField.FieldName} {{{IdField.GetCodeParamName()}}} exists."");
            }}

            return new {TableAlias}Model({TableVariableName});
        }}
";
        }

        private string CreateMethod()
        {
            return $@"        /// <summary>Creates the {TableVariableName}.</summary>
        /// <param name=""new{TableAlias}"">The {TableVariableName}s model.</param>
        /// <returns>new {TableVariableName}</returns>
        [HttpPost]
        public async Task<{TableAlias}Model> Create{TableAlias}(New{TableAlias}Model new{TableAlias})
        {{
            if (new{TableAlias} == null)
            {{
                throw new ArgumentNullException(nameof(new{TableAlias}));
            }}

            // TODO: Perform validation here

            // TODO: Initialize auto-populated fields here, e.g. create date

            var toInsert = new {TableAlias}Model(new{TableAlias});
            toInsert.{IdField.FieldName} = await {TableAlias}DataProvider.Insert(toInsert.ToData());
            return toInsert;
        }}
";
        }

        private string UpdateMethod()
        {
            return $@"        /// <summary>Updates the {TableVariableName}.</summary>
        /// <param name=""{IdField.GetCodeParamName()}""></param>
        /// <param name=""{TableVariableName}Model""></param>
        /// <returns>updated {TableVariableName}</returns>
        [Route(""{{{IdField.GetCodeParamName()}}}"")]
        [HttpPut]
        public async Task<{TableAlias}Model> Update{TableAlias}(int {IdField.GetCodeParamName()}, New{TableAlias}Model {TableVariableName}Model)
        {{
            if ({TableVariableName}Model == null)
            {{
                throw new ArgumentNullException(nameof({TableVariableName}Model));
            }}

            if ({IdField.GetCodeParamName()} <= 0)
            {{
                throw new ArgumentException(""The {TableVariableName} to update must have a valid {IdField.GetCodeParamName()}."");
            }}

            var toUpdate = await {TableAlias}DataProvider.GetBy{IdField.FieldName}({IdField.GetCodeParamName()});
            if (toUpdate == null)
            {{
                throw new HttpResponseException(404, $""No {TableVariableName} with ID {{{IdField.GetCodeParamName()}}} exists."");
            }}

            // TODO: Implement Validations here

            // Update fields that are allowed to be updated
            {GenUpdateFields()}
            
            // Perform the update
            var result = await {TableAlias}DataProvider.Update(toUpdate);
            if (result != 1)
            {{
                throw new InvalidOperationException($""{{result}} records were updated when it should have been one."");
            }}

            return new {TableAlias}Model(toUpdate);
        }}
";
        }

        private string GenUpdateFields()
        {
            var updateStatements = new List<string>();
            foreach (var field in Table.UpdateFields)
            {
                updateStatements.Add($"toUpdate.{field.FieldName} = {TableVariableName}Model.{field.FieldName};");
            }

            return string.Join($"{Environment.NewLine}            ", updateStatements);
        }

        private string DeleteMethod()
        {
            return $@"        /// <summary>Deletes the {TableVariableName} with the given {IdField.FieldName}.</summary>
        /// <param name=""{IdField.GetCodeParamName()}"">The {TableVariableName} {IdField.FieldName}.</param>
        [Route(""{{{IdField.GetCodeParamName()}}}"")]
        [HttpDelete]
        public async Task Delete{TableAlias}(int {IdField.GetCodeParamName()})
        {{
            await {TableAlias}DataProvider.Delete({IdField.GetCodeParamName()});
        }}
";
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}sController";
        }
    }
}

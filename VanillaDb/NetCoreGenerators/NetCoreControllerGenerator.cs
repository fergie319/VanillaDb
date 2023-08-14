﻿using System;
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
            return
                BeginClass() + Environment.NewLine +
                GetAllMethod() + Environment.NewLine +
                GetByIdMethod() + Environment.NewLine +
                CreateMethod() + Environment.NewLine +
                UpdateMethod() + Environment.NewLine +
                DeleteMethod() + Environment.NewLine +
                EndClass();
        }

        private string BeginClass()
        {
            return $@"using FleetRater.DataProviders.{TableAlias};
using FleetRater.Filters;
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
        public async Task<IEnumerable<{TableAlias}DataModel>> Get{TableAlias}s()
        {{
            var {TableVariableName}s = await {TableAlias}DataProvider.GetAll();
            return {TableVariableName}s;
        }}
";
        }

        private string GetByIdMethod()
        {
            return $@"        /// <summary>Gets the {TableVariableName} with the given ID.</summary>
        /// <returns>{TableAlias} with the given ID.</returns>
        [Route(""{{id}}"")]
        [HttpGet]
        public async Task<{TableAlias}DataModel> Get{TableAlias}(int id)
        {{
            var {TableVariableName} = await {TableAlias}DataProvider.GetById(id);
            if ({TableVariableName} == null)
            {{
                throw new HttpResponseException(404, $""No {TableVariableName} with ID {{id}} exists."");
            }}

            return {TableVariableName};
        }}
";
        }

        private string CreateMethod()
        {
            return $@"        /// <summary>Creates the {TableVariableName}.</summary>
        /// <param name=""new{TableAlias}"">The {TableVariableName}s model.</param>
        /// <returns>new {TableVariableName}</returns>
        [HttpPost]
        public async Task<{TableAlias}DataModel> Create{TableAlias}({TableAlias}DataModel new{TableAlias})
        {{
            if (new{TableAlias} == null)
            {{
                throw new ArgumentNullException(nameof(new{TableAlias}));
            }}

            if (string.IsNullOrEmpty(new{TableAlias}.Name))
            {{
                throw new InvalidOperationException($""{TableAlias} Name is a required property of the {TableAlias}."");
            }}

            var {TableVariableName}s = await {TableAlias}DataProvider.GetAll();
            if ({TableVariableName}s.Any(a => string.Equals(a.Name, new{TableAlias}.Name, StringComparison.InvariantCultureIgnoreCase)))
            {{
                throw new InvalidOperationException($""{TableAlias} Name {{new{TableAlias}.Name}} already exists."");
            }}

            new{TableAlias}.Id = await {TableAlias}DataProvider.Insert(new{TableAlias});
            return new{TableAlias};
        }}
";
        }

        private string UpdateMethod()
        {
            return $@"        /// <summary>Updates the {TableVariableName}.</summary>
        /// <param name=""id""></param>
        /// <param name=""{TableVariableName}Data""></param>
        /// <returns>updated {TableVariableName}</returns>
        [Route(""{{id}}"")]
        [HttpPut]
        public async Task<{TableAlias}DataModel> Update{TableAlias}(int id, {TableAlias}DataModel {TableVariableName}Data)
        {{
            if ({TableVariableName}Data == null)
            {{
                throw new ArgumentNullException(nameof({TableVariableName}Data));
            }}

            if (id <= 0)
            {{
                throw new ArgumentException(""The {TableVariableName} to update must have a valid id."");
            }}

            var toUpdate = await {TableAlias}DataProvider.GetById(id);
            if (toUpdate == null)
            {{
                throw new HttpResponseException(404, $""No {TableVariableName} with ID {{id}} exists."");
            }}

            // Validate that the updated {TableVariableName} name doesn't already exist
            if (!string.Equals({TableVariableName}Data.Name, toUpdate.Name, StringComparison.InvariantCultureIgnoreCase))
            {{
                // Verify the {TableVariableName} name against all other {TableVariableName}s except the one we're updating
                var {TableVariableName}s = await {TableAlias}DataProvider.GetAll();
                if ({TableVariableName}s
                    .Except({TableVariableName}s.Where(a => a.Id == id))
                    .Any(a => string.Equals(a.Name, {TableVariableName}Data.Name, StringComparison.InvariantCultureIgnoreCase)))
                {{
                    throw new InvalidOperationException($""{TableAlias} Name {{{TableVariableName}Data.Name}} already exists."");
                }}
            }}

            // Update fields that are allowed to be updated
            toUpdate.Name = {TableVariableName}Data.Name;

            // Perform the update
            var result = await {TableAlias}DataProvider.Update(toUpdate);
            if (result != 1)
            {{
                throw new InvalidOperationException($""{{result}} records were updated when it should have been one."");
            }}

            return toUpdate;
        }}
";
        }

        private string DeleteMethod()
        {
            return $@"
        /// <summary>Deletes the {TableVariableName} with the given Id.</summary>
        /// <param name=""{TableVariableName}Id"">The {TableVariableName} identifier.</param>
        [Route(""{{id}}"")]
        [HttpDelete]
        public async Task Delete{TableAlias}(int {TableVariableName}Id)
        {{
            await {TableAlias}DataProvider.Delete({TableVariableName}Id);
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

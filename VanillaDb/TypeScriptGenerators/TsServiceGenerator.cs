using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.TypeScriptGenerators
{
    /// <summary>Class for generating a TypeScript Axios service.</summary>
    public class TsServiceGenerator : ICodeTemplate
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
            var result =
$@"import axios from ""axios"";
import I{TableAlias}Model from ""../Models/I{TableAlias}Model""

export default class {TableAlias}sService {{

    Get{TableAlias}s = async () => {{
        let {TableVariableName}s: I{TableAlias}Model[];
        let url = ""/api/{TableAlias}s"";
        try {{
            const response = await axios.get<I{TableAlias}Model[]>(url);
            if (response.status == 200) {{
                {TableVariableName}s = response.data.map({TableVariableName} => {{
                    const parsed{TableAlias} = {{ ...{TableVariableName} }};
                    return parsed{TableAlias};
                }})
            }} else {{
                throw new Error(`Server returned a ${{response.status}}.`);
            }}
        }} catch (err: any) {{
            throw new Error(err.message);
        }}

        return {TableVariableName}s;
    }}

    Get{TableAlias} = async (id: number) => {{
        let {TableVariableName}: I{TableAlias}Model;
        let url = `/api/{TableAlias}s/${{id}}`;
        const response = await axios.get<I{TableAlias}Model>(url);
        if (response.status == 200) {{
            {TableVariableName} = response.data;
        }} else {{
            throw new Error(`Server returned a ${{response.status}}.`);
        }}

        return {TableVariableName};
    }}

    Insert{TableAlias} = async ({TableVariableName}: I{TableAlias}Model) => {{
        let url = `/api/{TableAlias}s`;
        const response = await axios.post<I{TableAlias}Model>(url, {TableVariableName});
        if (response.status == 200) {{
            {TableVariableName} = response.data;
        }} else {{
            throw new Error(`Server returned a ${{response.status}}.`);
        }}

        return {TableVariableName};
    }}

    Update{TableAlias} = async ({TableVariableName}: I{TableAlias}Model) => {{
        let url = `/api/{TableAlias}s`;
        const response = await axios.put<I{TableAlias}Model>(url, {TableVariableName});
        if (response.status == 200) {{
            {TableVariableName} = response.data;
        }} else {{
            throw new Error(`Server returned a ${{response.status}}.`);
        }}

        return {TableVariableName};
    }}

    Delete{TableAlias} = async (id: number) => {{
        let url = `/api/{TableAlias}s`;
        const response = await axios.delete<I{TableAlias}Model>(url);
        if (response.status !== 200) {{
            throw new Error(`Server returned a ${{response.status}}.`);
        }}
    }}
}}
";
            return result;
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}Service";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaDb.Models;

namespace VanillaDb.DataProviders
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.DataProviders.DataProviderInterfaceBase" />
    public partial class DataProviderInterface
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Initializes a new instance of the <see cref="DataProviderInterface" /> class.</summary>
        /// <param name="table">The table model.</param>
        /// <param name="indexes">The indexes.</param>
        public DataProviderInterface(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Class name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return $"I{Table.TableName}DataProvider";
        }

        /// <summary>Generates the insert method for the data provider.</summary>
        /// <returns>Insert method definition.</returns>
        public string GenerateInsertMethod()
        {
            /////// <summary>Inserts the given <#= Table.TableName #> data model into the <#= Table.TableName #> table.</summary>
            /////// <param name="<#= Table.TableName.ToLower() #> Data">The <#= Table.TableName.ToLower() #> data to insert.</param>
            /////// <returns>The ID of the inserted <#= Table.TableName #> record.</returns>
            ////int Insert(<#= Table.TableName #>DataModel <#= Table.TableName.ToCamelCase() #>Data);
            return string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.NetCoreGenerators
{
    /// <summary>Class for generating .NET Core Controllers and Models.</summary>
    public class NetCoreModelGenerator : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

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
                EndNamespace() + Environment.NewLine;
        }

        private string BeginNamespace()
        {
            return $@"using FleetRater.DataProviders.Account;

namespace FleetRater.Models
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
    /// <summary>Service Model for New Accounts</summary>
    public class NewAccountModel
    {{
{GenerateProperties(null)}
    }}
";
        }

        private string ModelClass()
        {
            return $@"
    /// <summary>Service Model for Accounts - inherits NewAccountModel and defines auto-populated fields.</summary>
    public class AccountModel : NewAccountModel
    {{
{GenerateProperties(null)}

{GenerateToMethods(null)}
    }}
";
        }

        private string GenerateProperties(IEnumerable<FieldModel> fields)
        {
            return $@"        /// <summary>Gets or sets the name.</summary>
        public string Name {{ get; set; }} = string.Empty;

        /// <summary>Gets or sets the Id.</summary>
        public int Id {{ get; set; }}

        /// <summary>Gets or sets the update date UTC.</summary>
        public DateTime UpdateDateUtc {{ get; set; }}";
        }
        private string GenerateToMethods(IEnumerable<FieldModel> fields)
        {
            return $@"        /// <summary>Converts the AccountDataModel to AccountModel.</summary>
        /// <param name=""accountData"">The account data to convert.</param>
        /// <returns>AccountModel instance.</returns>
        public AccountModel ToModel(AccountDataModel accountData)
        {{
            var result = new AccountModel();
            result.Id = accountData.Id;
            result.Name = accountData.Name;
            result.UpdateDateUtc = accountData.UpdateDateUtc;
            return result;
        }}

        /// <summary>Converts the AccountModel to AccountDataModel.</summary>
        /// <param name=""accountModel"">The account model to convert.</param>
        /// <returns>AccountData instance.</returns>
        public AccountDataModel ToData(AccountModel accountModel)
        {{
            var result = new AccountDataModel();
            result.Id = accountModel.Id;
            result.Name = accountModel.Name;
            result.UpdateDateUtc = accountModel.UpdateDateUtc;
            return result;
        }}";
        }

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}Model";
        }
    }
}

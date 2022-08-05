using VanillaDb.Models;

namespace VanillaDb.GetProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class GetAllStoredProc : ICodeTemplate
    {
        private TableModel Table { get; set; }

        /// <summary>Initializes a new instance of the <see cref="GetAllStoredProc" /> class.</summary>
        /// <param name="table">The table to query.</param>
        public GetAllStoredProc(TableModel table)
        {
            Table = table;
        }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "sql";

        /// <summary>Generates the portion of the procedure name that is composed of the indexed fields.</summary>
        /// <returns>Underscore-separated field names</returns>
        public string GenerateName()
        {
            return $"USP_{Table.TableName}_GetAll";
        }
    }
}

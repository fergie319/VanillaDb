namespace VanillaDb.Models
{
    /// <summary>Stores VanillaDb generation configuration settings.</summary>
    public class VanillaConfig
    {
        /// <summary>Gets or sets the path to a single table SQL file or a folder containing table definitions.</summary>
        public string TableSqlPath { get; set; }

        /// <summary>Gets or sets the folder path to write all output SQL files.</summary>
        public string OutputSqlPath { get; set; }

        /// <summary>Gets or sets the output code path.</summary>
        public string OutputCodePath { get; set; }

        /// <summary>Gets or sets the code namespace.</summary>
        public string CodeNamespace { get; set; }

        /// <summary>Gets or sets the name of the company used in file headers.</summary>
        public string CompanyName { get; set; }
    }
}

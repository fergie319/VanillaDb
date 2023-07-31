using System.Collections.Generic;
using System.Runtime.Serialization;
using VanillaDb.Configuration;

namespace VanillaDb.Models
{
    /// <summary>Stores VanillaDb generation configuration settings.</summary>
    [DataContract]
    public class VanillaConfig
    {
        /// <summary>Gets or sets the path to a single table SQL file or a folder containing table definitions.</summary>
        [DataMember()]
        public string TableSqlPath { get; set; }

        /// <summary>Gets or sets the folder path to write all output SQL files.</summary>
        [DataMember()]
        public string OutputSqlPath { get; set; }

        /// <summary>Gets or sets the output code path.</summary>
        [DataMember()]
        public string OutputCodePath { get; set; }

        /// <summary>Gets or sets the output path for C# Controllers code.</summary>
        [DataMember()]
        public string OutputControllersPath { get; set; }

        /// <summary>Gets or sets the output path for JavaScript Services code.</summary>
        [DataMember()]
        public string OutputJsServicesPath { get; set; }

        /// <summary>Gets or sets the code namespace.</summary>
        [DataMember()]
        public string CodeNamespace { get; set; }

        /// <summary>Gets or sets the name of the company used in file headers.</summary>
        [DataMember()]
        public string CompanyName { get; set; }

        /// <summary>Gets or sets the individual table configuration settings.</summary>
        public IEnumerable<TableConfig> TableConfigs { get; set; }
    }
}

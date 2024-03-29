﻿using System;
using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb.DataProviders
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.DataProviders.DataProviderInterfaceBase" />
    public partial class DataProviderInterface : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "cs";

        /// <summary>Initializes a new instance of the <see cref="DataProviderInterface" /> class.</summary>
        /// <param name="table">The table model.</param>
        /// <param name="indexes">The indexes.</param>
        public DataProviderInterface(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Gets the primary key for the table model - throws exception if more than one primary key exists.</summary>
        /// <value>The primary key.</value>
        public FieldModel PrimaryKey
        {
            get { return Table.PrimaryKey; }
        }

        /// <summary>Gets the name of the record type being worked with (the table name).</summary>
        public string Record
        {
            get { return Table.TableAlias; }
        }

        /// <summary>Gets the lowercase name of the record type being worked with (the table name).</summary>
        public string RecordLower
        {
            get { return Table.TableAlias.ToLower(); }
        }

        /// <summary>Gets the camelCase name of the record type being worked with (the table name).</summary>
        public string RecordCamel
        {
            get { return Table.TableAlias.ToLower(); }
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Class name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return $"I{Table.TableAlias}DataProvider";
        }

        /// <summary>Generates the insert method for the data provider.</summary>
        /// <returns>Insert method definition.</returns>
        public string GenerateInsertMethod()
        {
            var indent = "        ";
            return
            $"/// <summary>Inserts the given {Record} data model into the {Record} table.</summary>{Environment.NewLine}" +
            $"{indent}/// <param name=\"{RecordCamel}Data\">The {RecordLower} data to insert.</param>{Environment.NewLine}" +
            $"{indent}/// <returns>The ID of the inserted {Record} record.</returns>{Environment.NewLine}" +
            $"{indent}Task<int> Insert({Record}DataModel {RecordCamel}Data);";
        }

        /// <summary>Generates the insert method for the data provider.</summary>
        /// <returns>Insert method definition.</returns>
        public string GenerateUpdateMethod()
        {
            var indent = "        ";
            return
            $"/// <summary>Updates the given {Record} data model in the {Record} table.</summary>{Environment.NewLine}" +
            $"{indent}/// <param name=\"{RecordCamel}Data\">The {RecordLower} data to update.</param>{Environment.NewLine}" +
            $"{indent}/// <returns>The number of records affected by the update.</returns>{Environment.NewLine}" +
            $"{indent}Task<int> Update({Record}DataModel {RecordCamel}Data);";
        }

        /// <summary>Gets the method parameters for the GetAll dataprovider method.</summary>
        /// <returns>Method params string</returns>
        public string GetAllMethodParams()
        {
            // Create a fake index so that Temporal parameters are generated
            var index = new IndexModel() { Fields = new List<FieldModel>(), Table = Table };
            return index.GetByIndexMethodParams();
        }

        /// <summary>Gets the xml parameter comments for the GetAll dataprovider method.</summary>
        /// <returns></returns>
        public string GetAllParamsXmlComments()
        {
            // Create a fake index so that Temporal parameters are generated
            var index = new IndexModel() { Fields = new List<FieldModel>(), Table = Table };
            var result = index.GetByIndexParamsXmlComments();
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = $"{Environment.NewLine}        {result}";
            }

            return result;
        }
    }
}

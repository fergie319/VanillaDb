﻿using System;
using System.Collections.Generic;
using System.Linq;
using VanillaDb.Models;
using VanillaDb.TypeTables;

namespace VanillaDb.DataProviders
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.DataProviders.SqlDataProviderBase" />
    public partial class SqlDataProvider : ICodeTemplate
    {
        private TableModel Table { get; set; }

        private IEnumerable<IndexModel> Indexes { get; set; }

        /// <summary>Initializes a new instance of the <see cref="DataProviderInterface" /> class.</summary>
        /// <param name="table">The table model.</param>
        /// <param name="indexes">The indexes.</param>
        public SqlDataProvider(TableModel table, IEnumerable<IndexModel> indexes)
        {
            Table = table;
            Indexes = indexes;
        }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "cs";

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

        /// <summary>Gets the name of the primary key field.</summary>
        public FieldModel PrimaryKey
        {
            get { return Table.PrimaryKey; }
        }

        /// <summary>Gets the index for the primary key.</summary>
        public IndexModel PrimaryKeyIndex
        {
            get { return Indexes.Single(i => i.IsPrimaryKey); }
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Class name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return $"{Table.TableAlias}SqlDataProvider";
        }

        /// <summary>Generates the insert proc parameters.</summary>
        /// <returns>Code to add parameters for invoking insert stored proc.</returns>
        public string GenerateInsertProcParams()
        {
            var indent = "                    ";
            var recordParam = $"{RecordCamel}Data";
            var parameters = Table.InsertFields
                .Select(f =>
                {
                    return (f.IsNullable)
                        ? $"\"@{f.FieldName.ToCamelCase()}\", {recordParam}.{f.FieldName} ?? (object)DBNull.Value"
                        : $"\"@{f.FieldName.ToCamelCase()}\", {recordParam}.{f.FieldName}";
                });

            var addParameters = parameters.Select(p => $"command.Parameters.AddWithValue({p});");

            return string.Join($"{Environment.NewLine}{indent}", addParameters);
        }

        /// <summary>Generates the update proc parameters.</summary>
        /// <returns>Code to add parameters for invoking update stored proc.</returns>
        public string GenerateUpdateProcParams()
        {
            var indent = "                    ";
            var recordParam = $"{RecordCamel}Data";

            var updateProcFields = Table.UpdateFields;
            if (!Table.UpdateFields.Any(f => f.IsPrimaryKey))
            {
                updateProcFields = updateProcFields.Prepend(PrimaryKey);
            }
            var parameters = updateProcFields
                .Select(f =>
                {
                    return (f.IsNullable)
                        ? $"\"{f.GetParamName()}\", {recordParam}.{f.FieldName} ?? (object)DBNull.Value"
                        : $"\"{f.GetParamName()}\", {recordParam}.{f.FieldName}";
                });

            var addParameters = parameters.Select(p => $"command.Parameters.AddWithValue({p});");

            return string.Join($"{Environment.NewLine}{indent}", addParameters);
        }

        /// <summary>Generates the lines for reading each field from a db reader.</summary>
        /// <returns>Code to read each field and populate the data model.</returns>
        public string GenerateReadFields()
        {
            var indent = "            ";
            var readLines = Table.Fields.Select(f =>
            {
                return (f.IsNullable)
                    ? $"data.{f.FieldName} = (reader[\"{f.FieldName}\"] != DBNull.Value) ? ({f.FieldType.GetAliasOrName()})reader[\"{f.FieldName}\"] : null;"
                    : $"data.{f.FieldName} = ({f.FieldType.GetAliasOrName()})reader[\"{f.FieldName}\"];";
            });

            return string.Join($"{Environment.NewLine}{indent}", readLines);
        }

        /// <summary>Generates the code to add parameters for the get by index procedure.</summary>
        /// <returns></returns>
        public string GenerateGetByIndexAddParametersCode(IEnumerable<FieldModel> fields)
        {
            var indent = "                    ";
            var addParamsLines = fields.Select(f => f.GetAddParamCode());
            return string.Join($"{Environment.NewLine}{indent}", addParamsLines);
        }

        /// <summary>Generates the in memory data columns creation code.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateInMemoryDataColumns(IEnumerable<FieldModel> fields)
        {
            var indent = "            ";
            var columnCode = fields.Select(f => $"idDataTable.Columns.Add(new DataColumn(\"{f.FieldName}\", typeof({f.FieldType.GetAliasOrName()})));");
            return string.Join($"{Environment.NewLine}{indent}", columnCode);
        }

        /// <summary>Generates the in memory row creation code.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateInMemoryRowCreation(IEnumerable<FieldModel> fields)
        {
            var rows = fields.Select(f => $"{f.FieldName.ToCamelCase()}s.ElementAt(i)");
            var rowParams = string.Join(", ", rows);
            return $"idDataTable.Rows.Add({rowParams});";
        }

        /// <summary>Generates the GetBy(Bulk) stored procedure's parameter name.</summary>
        /// <returns>Newline separated stored procedure parameters</returns>
        public string GenerateBulkProcParameter(IndexModel index)
        {
            var typeTable = new TypeTable(index);
            var fieldNames = index.Fields.Select(f => f.FieldName.ToCamelCase());
            return $"@{string.Join("_", fieldNames)}s";
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

        /// <summary>Gets the name of the GetAll stored procedure.</summary>
        /// <param name="temporalType">Type of the temporal.</param>
        /// <returns></returns>
        public string GetAllProcName(TemporalTypes temporalType)
        {
            var procName = $"USP_{Table.TableName}_GetAll";
            if (Table.IsTemporal && temporalType == TemporalTypes.AsOf)
            {
                procName += "_AsOf";
            }
            else if (temporalType == TemporalTypes.All)
            {
                procName += "_All";
            }

            return procName;
        }
    }
}

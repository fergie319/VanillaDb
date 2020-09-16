﻿using System;
using System.Collections.Generic;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.DataProviders
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    /// <seealso cref="VanillaDb.DataProviders.SqlDataProviderBase" />
    public partial class SqlDataProvider
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

        /// <summary>Gets the name of the record type being worked with (the table name).</summary>
        public string Record
        {
            get { return Table.TableName; }
        }

        /// <summary>Gets the lowercase name of the record type being worked with (the table name).</summary>
        public string RecordLower
        {
            get { return Table.TableName.ToLower(); }
        }

        /// <summary>Gets the camelCase name of the record type being worked with (the table name).</summary>
        public string RecordCamel
        {
            get { return Table.TableName.ToLower(); }
        }

        /// <summary>Gets the name of the primary key field.</summary>
        public string PrimaryKey
        {
            get { return Table.Fields.First(f => f.IsPrimaryKey).FieldName; }
        }

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Class name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return $"{Table.TableName}SqlDataProvider";
        }

        /// <summary>Generates the name of the insert stored proc.</summary>
        /// <returns>Insert Stored proce name</returns>
        public string GenerateInsertProcName()
        {
            return $"USP_{Table.TableName}_Insert";
        }


        /// <summary>Generates the insert proc parameters.</summary>
        /// <returns>Code to add parameters for invoking insert stored proc.</returns>
        public string GenerateInsertProcParams()
        {
            var indent = "                    ";
            var recordParam = $"{Table.TableName.ToCamelCase()}Data";
            var parameters = Table.Fields
                .Where(f => !f.IsIdentity)
                .Select(f =>
                {
                    return (f.IsNullable)
                        ? $"\"@{f.FieldName.ToCamelCase()}\", {recordParam}.{f.FieldName} ?? (object)DBNull.Value"
                        : $"\"@{f.FieldName.ToCamelCase()}\", {recordParam}.{f.FieldName}";
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
                    ? $"data.{f.FieldName} = (reader[\"{f.FieldName}\"] != DBNull.Value) ? ({f.FieldType.FieldType.GetAliasOrName()})reader[\"{f.FieldName}\"] : null;"
                    : $"data.{f.FieldName} = ({f.FieldType.FieldType.GetAliasOrName()})reader[\"{f.FieldName}\"];";
            });

            return string.Join($"{Environment.NewLine}{indent}", readLines);
        }

        /// <summary>Creates a readable list of the given fields' names (joined by 'and').</summary>
        /// <param name="fields">The fields.</param>
        /// <returns>Human readable list of fields.</returns>
        public string ReadableFields(IEnumerable<FieldModel> fields)
        {
            return string.Join(" and ", fields.Select(f => f.FieldName));
        }

        /// <summary>Generates the get by index parameters XML comments.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateGetByIndexParamsXmlComments(IEnumerable<FieldModel> fields)
        {
            var indent = "        ";
            var xmlParams = fields.Select(f => $"/// <param name=\"{f.FieldName.ToCamelCase()}\">The {f.FieldName} value.</param>");
            return string.Join($"{Environment.NewLine}{indent}", xmlParams);
        }

        /// <summary>Generates the get by index method parameters.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateGetByIndexMethodParams(IEnumerable<FieldModel> fields)
        {
            return string.Join(", ", fields.Select(f => $"{f.FieldType.FieldType} {f.FieldName.ToCamelCase()}"));
        }

        /// <summary>Generates the name of the get by index method.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateGetByIndexMethodName(IEnumerable<FieldModel> fields)
        {
            return $"GetBy{string.Join("And", fields.Select(f => f.FieldName))}";
        }

        /// <summary>Generates the name of the get by index stored proc.</summary>
        /// <returns></returns>
        public string GenerateGetByIndexStoredProcName(IndexModel index)
        {
            var procNameFields = index.Fields.Select(f => f.FieldName);
            return $"USP_{Table.TableName}_GetBy{string.Join("_", procNameFields)}";
        }

        /// <summary>Generates the code to add parameters for the get by index procedure.</summary>
        /// <returns></returns>
        public string GenerateGetByIndexAddParametersCode(IEnumerable<FieldModel> fields)
        {
            var indent = "                    ";
            var addParamsLines = fields.Select(f => $"command.Parameters.AddWithValue(\"@{f.FieldName.ToCamelCase()}\", {f.FieldName.ToCamelCase()});");
            return string.Join($"{Environment.NewLine}{indent}", addParamsLines);
        }

        /// <summary>Generates the get by index bulk parameters XML comments.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateGetByIndexBulkParamsXmlComments(IEnumerable<FieldModel> fields)
        {
            var indent = "        ";
            var xmlParams = fields.Select(f => $"/// <param name=\"{f.FieldName.ToCamelCase()}s\">The {f.FieldName.ToCamelCase()} values.</param>");
            return string.Join($"{Environment.NewLine}{indent}", xmlParams);
        }

        /// <summary>Generates the get by index method parameters.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public string GenerateGetByIndexBulkMethodParams(IEnumerable<FieldModel> fields)
        {
            return string.Join(", ", fields.Select(f => $"IEnumerable<{f.FieldType.FieldType}> {f.FieldName.ToCamelCase()}"));
        }
    }
}

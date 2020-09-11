﻿using System;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.InsertProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class InsertStoredProc
    {
        private TableModel Table { get; set; }

        /// <summary>Initializes a new instance of the <see cref="InsertStoredProc"/> class.</summary>
        /// <param name="table">The table.</param>
        public InsertStoredProc(TableModel table)
        {
            Table = table;
        }

        /// <summary>Generates the stored procedure's parameter list.</summary>
        /// <returns></returns>
        public string GenerateProcParameters()
        {
            var insertParams = Table.Fields
                .Where(f => !f.IsIdentity)
                .Select(f => $"    @{f.FieldName.ToCamelCase()} {f.FieldType.SqlType}");
            return string.Join("," + Environment.NewLine, insertParams);
        }

        /// <summary>Generates the insert parameters for the stored procedure.</summary>
        /// <returns>Comma-separated list of non-identity fields.</returns>
        public string GenerateInsertParameters()
        {
            var insertParams = Table.Fields
                .Where(f => !f.IsIdentity)
                .Select(f => f.FieldName);
            return string.Join(", ", insertParams);
        }

        /// <summary>Generates the fields for the OUTPUT clause of the Insert statement.</summary>
        /// <returns></returns>
        public string GenerateOutputFields()
        {
            var insertParams = Table.Fields.Select(f => "INSERTED." + f.FieldName);
            return string.Join(", ", insertParams);
        }

        /// <summary>Generates the fields for the VALUES (...) clause of the Insert statement.</summary>
        /// <returns></returns>
        public string GenerateValuesFields()
        {
            var insertParams = Table.Fields
                .Where(f => !f.IsIdentity)
                .Select(f => "@" + f.FieldName.ToCamelCase());
            return string.Join(", ", insertParams);
        }
    }
}

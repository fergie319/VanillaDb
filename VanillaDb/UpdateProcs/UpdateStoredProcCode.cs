using System;
using System.Linq;
using VanillaDb.Models;

namespace VanillaDb.UpdateProcs
{
    /// <summary>Partial class supplies parameters to the T4 Template.</summary>
    public partial class UpdateStoredProc : ICodeTemplate
    {
        private TableModel Table { get; set; }

        /// <summary>Initializes a new instance of the <see cref="UpdateStoredProc"/> class.</summary>
        /// <param name="table">The table.</param>
        public UpdateStoredProc(TableModel table)
        {
            Table = table;
        }

        /// <summary>Gets the primary key.</summary>
        /// <value>The primary key.</value>
        public FieldModel PrimaryKey
        {
            get { return Table.PrimaryKey; }
        }

        /// <summary>Gets the file extension.</summary>
        public string FileExtension => "sql";

        /// <summary>Gets the name of the stored procedure.</summary>
        /// <returns>Stored Procedure name and file name (without .sql).</returns>
        public string GenerateName()
        {
            return Table.GetUpdateProcName();
        }

        /// <summary>Generates the stored procedure's parameter list.</summary>
        /// <returns></returns>
        public string GenerateProcParameters()
        {
            var updateProcFields = Table.UpdateFields;
            if (!Table.UpdateFields.Any(f => f.IsPrimaryKey))
            {
                updateProcFields = updateProcFields.Prepend(PrimaryKey);
            }

            var updateParams = updateProcFields.Select(f => $"    {f.GetParamName()} {f.FieldType.SqlType}");
            return string.Join("," + Environment.NewLine, updateParams);
        }

        /// <summary>Generates the fields for the OUTPUT clause of the Update statement.</summary>
        /// <returns></returns>
        public string GenerateSetFields()
        {
            var setStatements = Table.UpdateFields
                .Where(f => !f.IsIdentity)
                .Select(f => $"{f.FieldName} = {f.GetParamName()}");
            var timeStampValues = Table.UpdateTimeStampFields.Select(f => $"{f.FieldName} = GETUTCDATE()");
            setStatements = setStatements.Concat(timeStampValues);
            return string.Join(", ", setStatements);
        }
    }
}

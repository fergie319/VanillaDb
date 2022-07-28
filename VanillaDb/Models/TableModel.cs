using System;
using System.Collections.Generic;
using System.Linq;
using VanillaDb.Configuration;

namespace VanillaDb.Models
{
    /// <summary>Contains details about a table - name and fields</summary>
    public class TableModel
    {
        /// <summary>Gets the time stamp field names.</summary>
        /// <value>The time stamp field names.</value>
        public readonly IEnumerable<string> TimeStampFieldNames = new[] { "CreatedOnUtc", "UpdatedOnUtc" };

        /// <summary>The update time stamp field names</summary>
        public readonly IEnumerable<string> UpdateTimeStampFieldNames = new[] { "UpdatedOnUtc" };

        /// <summary>Gets or sets the configuration settings for this table.</summary>
        public TableConfig Config { get; set; }

        /// <summary>Gets or sets the company.</summary>
        public string Company { get; set; }

        /// <summary>Gets or sets the namespace.</summary>
        public string Namespace { get; set; }

        /// <summary>Gets or sets the table name.</summary>
        public string TableName { get; set; }

        /// <summary>Gets the table alias - used for naming models, dataproviders, and interfaces.</summary>
        public string TableAlias { get { return Config?.TableAlias; } }

        /// <summary>Gets or sets the schema.</summary>
        public string Schema { get; set; }

        /// <summary>Gets or sets a whether this is a Temporal table.</summary>
        public bool IsTemporal { get; set; }

        /// <summary>Gets or sets the fields.</summary>
        public IList<FieldModel> Fields { get; set; }

        /// <summary>Gets or sets the data dictionary for the table.</summary>
        public IEnumerable<DictionaryDefinitionModel> DataDictionary { get; set; }

        /// <summary>Gets or sets a value indicating whether this table has a primary key.</summary>
        public bool HasPrimaryKey { get { return PrimaryKey != null; } }

        /// <summary>Gets the primary key for the table model - throws exception if more than one primary key exists.</summary>
        /// <value>The primary key.</value>
        public FieldModel PrimaryKey
        {
            get { return Fields.SingleOrDefault(f => f.IsPrimaryKey); }
        }

        /// <summary>Gets the insert fields.</summary>
        /// <value>The insert fields.</value>
        public IEnumerable<FieldModel> InsertFields
        {
            get { return Fields.Where(f => !f.IsIdentity && !f.IsTemporalField && !TimeStampFieldNames.Contains(f.FieldName)); }
        }

        /// <summary>Gets the update fields.</summary>
        /// <value>The update fields.</value>
        public IEnumerable<FieldModel> UpdateFields
        {
            get { return Fields.Where(f => !f.IsIdentity && !f.IsTemporalField && !TimeStampFieldNames.Contains(f.FieldName)); }
        }

        /// <summary>Gets the update time stamp fields.</summary>
        /// <value>The update time stamp fields.</value>
        public IEnumerable<FieldModel> UpdateTimeStampFields
        {
            get { return Fields.Where(f => UpdateTimeStampFieldNames.Contains(f.FieldName)); }
        }

        /// <summary>Gets the definition for the given field if the table has a data dictionary.</summary>
        /// <param name="field">The field.</param>
        /// <returns>string definition - or empty string</returns>
        public string GetDefinition(FieldModel field)
        {
            var entry = DataDictionary.FirstOrDefault(d => string.Equals(d.Field, field.FieldName, StringComparison.InvariantCultureIgnoreCase));
            var result = entry?.Description ?? string.Empty;
            return result;
        }

        /// <summary>Gets the values for the given field if the table has a data dictionary.</summary>
        /// <param name="field">The field.</param>
        /// <returns>string of values - or empty string</returns>
        public string GetValues(FieldModel field)
        {
            var entry = DataDictionary.FirstOrDefault(d => string.Equals(d.Field, field.FieldName, StringComparison.InvariantCultureIgnoreCase));
            var result = entry?.ParsedValues ?? string.Empty;
            return result;
        }

        /// <summary>Gets the remarks for the given field (formatted string of definition + values) if the table has a data dictionary.</summary>
        /// <param name="field">The field.</param>
        /// <param name="indent">The indent string so that remarks lines up with the other comments.</param>
        /// <returns>string of remarks - or empty string</returns>
        public string GetRemarks(FieldModel field, string indent)
        {
            var remarks = string.Empty;
            var definition = GetDefinition(field);
            if (!string.IsNullOrEmpty(definition))
            {
                var splitDefinition = definition.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                remarks = $"{indent}/// Definition: {string.Join($"{Environment.NewLine}{indent}/// ", splitDefinition)}";
            }

            var values = GetValues(field);
            if (!string.IsNullOrEmpty(values))
            {
                if (!string.IsNullOrEmpty(remarks))
                {
                    // Add a newline if there was a definition (remarks is not null)
                    remarks += Environment.NewLine;
                }

                remarks += $"{indent}/// Possible Values: {GetValues(field)}";
            }

            // Replace special characters with xml-safe characters
            remarks = remarks
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");

            return remarks;
        }

        /// <summary>Gets the name of the generated data model for this table.</summary>
        /// <returns></returns>
        public string GetDataModelName()
        {
            return $"{TableAlias}DataModel";
        }

        /// <summary>Generates the name of the GetAll stored proc.</summary>
        /// <returns>Insert Stored proc name</returns>
        public string GetGetAllProcName()
        {
            return $"USP_{TableName}_GetAll";
        }

        /// <summary>Generates the name of the insert stored proc.</summary>
        /// <returns>Insert Stored proc name</returns>
        public string GetInsertProcName()
        {
            return $"USP_{TableName}_Insert";
        }

        /// <summary>Generates the name of the update stored proc.</summary>
        /// <returns>Update Stored proc name</returns>
        public string GetUpdateProcName()
        {
            return $"USP_{TableName}_Update";
        }

        /// <summary>Gets the name of the delete stored procedure.</summary>
        /// <returns>Stored procedure name</returns>
        public string GetDeleteProcName()
        {
            return $"USP_{TableName}_Delete";
        }

        /// <summary>Gets the name of the delete stored procedure.</summary>
        /// <returns>Stored procedure name</returns>
        public string GetDeleteBulkProcName()
        {
            return $"USP_{TableName}_DeleteBulk";
        }
    }
}

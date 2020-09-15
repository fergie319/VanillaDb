﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace VanillaDb.DataProviders
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using VanillaDb;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class SqlDataProvider : SqlDataProviderBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("// <copyright file=\"");
            
            #line 8 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateName()));
            
            #line default
            #line hidden
            this.Write(".cs\" company=\"MMF Software Developers Inc.\">\r\n// Copyright (c) MMF Software Devel" +
                    "opers Inc.. All rights reserved.\r\n// </copyright>\r\n\r\nnamespace ");
            
            #line 12 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("Service.DataProviders\r\n{\r\n    using System;\r\n    using System.Collections.Generic" +
                    ";\r\n    using System.Data;\r\n    using System.Data.SqlClient;\r\n    using System.Li" +
                    "nq;\r\n\r\n    /// <summary>SQL implementation of data provider interface for ");
            
            #line 20 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" data.</summary>\r\n    /// <seealso cref=\"");
            
            #line 21 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("Service.DataProviders.I");
            
            #line 21 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("DataProvider\" />\r\n    public partial class ");
            
            #line 22 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateName()));
            
            #line default
            #line hidden
            this.Write(" : I");
            
            #line 22 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("DataProvider\r\n    {\r\n        /// <summary>Initializes a new instance of the <see " +
                    "cref=\"");
            
            #line 24 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateName()));
            
            #line default
            #line hidden
            this.Write("\"/> class.</summary>\r\n        /// <param name=\"connectionString\">The connection s" +
                    "tring.</param>\r\n        public ");
            
            #line 26 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateName()));
            
            #line default
            #line hidden
            this.Write("(string connectionString) // TODO: Add Logger and log statements\r\n        {\r\n    " +
                    "        this.ConnectionString = connectionString;\r\n        }\r\n\r\n        /// <sum" +
                    "mary>Gets or sets the connection string.</summary>\r\n        /// <value>The conne" +
                    "ction string.</value>\r\n        private string ConnectionString { get; set; }\r\n\r\n" +
                    "        /// <summary>Gets the data for the book with the given BookId.</summary>" +
                    "\r\n        /// <param name=\"bookId\">The book Id.</param>\r\n        /// <returns>Bo" +
                    "ok Data Model or null if not found.</returns>\r\n        /// <exception cref=\"Inva" +
                    "lidOperationException\">${bookId} is not a valid BookId.</exception>\r\n        pub" +
                    "lic BookDataModel GetByBookId(int bookId)\r\n        {\r\n            if (bookId < 1" +
                    ")\r\n            {\r\n                throw new InvalidOperationException($\"${bookId" +
                    "} is not a valid BookId.\");\r\n            }\r\n\r\n            BookDataModel bookData" +
                    " = null;\r\n            using (var connection = new SqlConnection(this.ConnectionS" +
                    "tring))\r\n            {\r\n                using (var command = new SqlCommand(\"USP" +
                    "_Book_GetByBookId\"))\r\n                {\r\n                    command.CommandType" +
                    " = CommandType.StoredProcedure;\r\n                    command.Connection = connec" +
                    "tion;\r\n                    command.Parameters.AddWithValue(\"@bookId\", bookId);\r\n" +
                    "\r\n                    connection.Open();\r\n                    var reader = comma" +
                    "nd.ExecuteReader();\r\n                    if (reader.HasRows)\r\n                  " +
                    "  {\r\n                        while (reader.Read())\r\n                        {\r\n " +
                    "                           bookData = ParseBookDataModel(reader);\r\n             " +
                    "           }\r\n                    }\r\n                }\r\n            }\r\n\r\n       " +
                    "     return bookData;\r\n        }\r\n\r\n        /// <summary>Gets the data for the b" +
                    "ook with the given BookId.</summary>\r\n        /// <param name=\"bookIds\">The book" +
                    " Ids.</param>\r\n        /// <returns>Book Data Model or null if not found.</retur" +
                    "ns>\r\n        /// <exception cref=\"System.ArgumentNullException\">bookIds</excepti" +
                    "on>\r\n        public IEnumerable<BookDataModel> GetByBookId(IEnumerable<int> book" +
                    "Ids)\r\n        {\r\n            if (bookIds == null || !bookIds.Any())\r\n           " +
                    " {\r\n                throw new ArgumentNullException(nameof(bookIds));\r\n         " +
                    "   }\r\n\r\n            // Create an in-memory datatable with all of the BookIds\r\n  " +
                    "          var idDataTable = new DataTable();\r\n            idDataTable.Columns.Ad" +
                    "d(new DataColumn(\"BookId\", typeof(int)));\r\n            foreach (var bookId in bo" +
                    "okIds)\r\n            {\r\n                idDataTable.Rows.Add(bookId);\r\n          " +
                    "  }\r\n\r\n            var bookData = new List<BookDataModel>();\r\n            using " +
                    "(var connection = new SqlConnection(this.ConnectionString))\r\n            {\r\n    " +
                    "            using (var command = new SqlCommand(\"USP_Book_GetByBookId_Multiple\")" +
                    ")\r\n                {\r\n                    command.CommandType = CommandType.Stor" +
                    "edProcedure;\r\n                    command.Connection = connection;\r\n\r\n          " +
                    "          // Create the Stored Procedure Parameter and set to Type_IdTable\r\n    " +
                    "                var idsParam = command.Parameters.AddWithValue(\"@bookIds\", idDat" +
                    "aTable);\r\n                    idsParam.SqlDbType = SqlDbType.Structured;\r\n      " +
                    "              idsParam.TypeName = \"dbo.Type_BookId_Table\";\r\n\r\n                  " +
                    "  connection.Open();\r\n                    var reader = command.ExecuteReader();\r" +
                    "\n                    if (reader.HasRows)\r\n                    {\r\n               " +
                    "         while (reader.Read())\r\n                        {\r\n                     " +
                    "       var data = ParseBookDataModel(reader);\r\n                            bookD" +
                    "ata.Add(data);\r\n                        }\r\n                    }\r\n              " +
                    "  }\r\n            }\r\n\r\n            return bookData;\r\n        }\r\n\r\n        /// <su" +
                    "mmary>Gets the data for the book with the given ISBN13.</summary>\r\n        /// <" +
                    "param name=\"isbn13\">The isbn13 value.</param>\r\n        /// <returns>Book Data Mo" +
                    "del or null if not found</returns>\r\n        /// <exception cref=\"System.Argument" +
                    "NullException\">isbn13</exception>\r\n        public BookDataModel GetByISBN13(stri" +
                    "ng isbn13)\r\n        {\r\n            if (string.IsNullOrWhiteSpace(isbn13))\r\n     " +
                    "       {\r\n                throw new ArgumentNullException(nameof(isbn13));\r\n    " +
                    "        }\r\n\r\n            BookDataModel bookData = null;\r\n            using (var " +
                    "connection = new SqlConnection(this.ConnectionString))\r\n            {\r\n         " +
                    "       using (var command = new SqlCommand(\"USP_Book_GetByISBN13\"))\r\n           " +
                    "     {\r\n                    command.CommandType = CommandType.StoredProcedure;\r\n" +
                    "                    command.Connection = connection;\r\n                    comman" +
                    "d.Parameters.AddWithValue(\"@isbn13\", isbn13);\r\n\r\n                    connection." +
                    "Open();\r\n                    var reader = command.ExecuteReader();\r\n            " +
                    "        if (reader.HasRows)\r\n                    {\r\n                        whil" +
                    "e (reader.Read())\r\n                        {\r\n                            bookDa" +
                    "ta = ParseBookDataModel(reader);\r\n                        }\r\n                   " +
                    " }\r\n                }\r\n            }\r\n\r\n            return bookData;\r\n        }\r" +
                    "\n\r\n        /// <summary>Gets the data for the book with the given ISBN13.</summa" +
                    "ry>\r\n        /// <param name=\"isbn13s\">The isbn13 values.</param>\r\n        /// <" +
                    "returns>Book Data Model or null if not found.</returns>\r\n        /// <exception " +
                    "cref=\"System.ArgumentNullException\">isbn13s</exception>\r\n        public IEnumera" +
                    "ble<BookDataModel> GetByISBN13(IEnumerable<string> isbn13s)\r\n        {\r\n        " +
                    "    if (isbn13s == null || !isbn13s.Any())\r\n            {\r\n                throw" +
                    " new ArgumentNullException(nameof(isbn13s));\r\n            }\r\n\r\n            // Cr" +
                    "eate an in-memory datatable with all of the BookIds\r\n            var idDataTable" +
                    " = new DataTable();\r\n            idDataTable.Columns.Add(new DataColumn(\"ISBN13\"" +
                    ", typeof(string)));\r\n            foreach (var bookId in isbn13s)\r\n            {\r" +
                    "\n                idDataTable.Rows.Add(bookId);\r\n            }\r\n\r\n            var" +
                    " bookData = new List<BookDataModel>();\r\n            using (var connection = new " +
                    "SqlConnection(this.ConnectionString))\r\n            {\r\n                using (var" +
                    " command = new SqlCommand(\"USP_Book_GetByISBN13_Multiple\"))\r\n                {\r\n" +
                    "                    command.CommandType = CommandType.StoredProcedure;\r\n        " +
                    "            command.Connection = connection;\r\n\r\n                    // Create th" +
                    "e Stored Procedure Parameter and set to Type_IdTable\r\n                    var id" +
                    "sParam = command.Parameters.AddWithValue(\"@isbn13s\", idDataTable);\r\n            " +
                    "        idsParam.SqlDbType = SqlDbType.Structured;\r\n                    idsParam" +
                    ".TypeName = \"dbo.Type_ISBN13_Table\";\r\n\r\n                    connection.Open();\r\n" +
                    "                    var reader = command.ExecuteReader();\r\n                    i" +
                    "f (reader.HasRows)\r\n                    {\r\n                        while (reader" +
                    ".Read())\r\n                        {\r\n                            var data = Pars" +
                    "eBookDataModel(reader);\r\n                            bookData.Add(data);\r\n      " +
                    "                  }\r\n                    }\r\n                }\r\n            }\r\n\r\n" +
                    "            return bookData;\r\n        }\r\n\r\n        /// <summary>Gets the data fo" +
                    "r the book with the given ASIN10.</summary>\r\n        /// <param name=\"asin10\">Th" +
                    "e asin10 value.</param>\r\n        /// <returns>Book Data Model or null if not fou" +
                    "nd</returns>\r\n        public BookDataModel GetByASIN10(string asin10)\r\n        {" +
                    "\r\n            if (string.IsNullOrWhiteSpace(asin10))\r\n            {\r\n           " +
                    "     throw new ArgumentNullException(nameof(asin10));\r\n            }\r\n\r\n        " +
                    "    BookDataModel bookData = null;\r\n            using (var connection = new SqlC" +
                    "onnection(this.ConnectionString))\r\n            {\r\n                using (var com" +
                    "mand = new SqlCommand(\"USP_Book_GetByASIN10\"))\r\n                {\r\n             " +
                    "       command.CommandType = CommandType.StoredProcedure;\r\n                    c" +
                    "ommand.Connection = connection;\r\n                    command.Parameters.AddWithV" +
                    "alue(\"@asin10\", asin10);\r\n\r\n                    connection.Open();\r\n            " +
                    "        var reader = command.ExecuteReader();\r\n                    if (reader.Ha" +
                    "sRows)\r\n                    {\r\n                        while (reader.Read())\r\n  " +
                    "                      {\r\n                            bookData = ParseBookDataMod" +
                    "el(reader);\r\n                        }\r\n                    }\r\n                }" +
                    "\r\n            }\r\n\r\n            return bookData;\r\n        }\r\n\r\n        /// <summa" +
                    "ry>Gets the data for the book with the given ASIN10.</summary>\r\n        /// <par" +
                    "am name=\"asin10s\">The asin10 values.</param>\r\n        /// <returns>Book Data Mod" +
                    "el or null if not found.</returns>\r\n        /// <exception cref=\"System.Argument" +
                    "NullException\">asin</exception>\r\n        public IEnumerable<BookDataModel> GetBy" +
                    "ASIN10(IEnumerable<string> asin10s)\r\n        {\r\n            if (asin10s == null " +
                    "|| !asin10s.Any())\r\n            {\r\n                throw new ArgumentNullExcepti" +
                    "on(nameof(asin10s));\r\n            }\r\n\r\n            // Create an in-memory datata" +
                    "ble with all of the BookIds\r\n            var idDataTable = new DataTable();\r\n   " +
                    "         idDataTable.Columns.Add(new DataColumn(\"ASIN10\", typeof(string)));\r\n   " +
                    "         foreach (var bookId in asin10s)\r\n            {\r\n                idDataT" +
                    "able.Rows.Add(bookId);\r\n            }\r\n\r\n            var bookData = new List<Boo" +
                    "kDataModel>();\r\n            using (var connection = new SqlConnection(this.Conne" +
                    "ctionString))\r\n            {\r\n                using (var command = new SqlComman" +
                    "d(\"USP_Book_GetByASIN10_Multiple\"))\r\n                {\r\n                    comm" +
                    "and.CommandType = CommandType.StoredProcedure;\r\n                    command.Conn" +
                    "ection = connection;\r\n\r\n                    // Create the Stored Procedure Param" +
                    "eter and set to Type_IdTable\r\n                    var idsParam = command.Paramet" +
                    "ers.AddWithValue(\"@asin10s\", idDataTable);\r\n                    idsParam.SqlDbTy" +
                    "pe = SqlDbType.Structured;\r\n                    idsParam.TypeName = \"dbo.Type_AS" +
                    "IN10_Table\";\r\n\r\n                    connection.Open();\r\n                    var " +
                    "reader = command.ExecuteReader();\r\n                    if (reader.HasRows)\r\n    " +
                    "                {\r\n                        while (reader.Read())\r\n              " +
                    "          {\r\n                            var data = ParseBookDataModel(reader);\r" +
                    "\n                            bookData.Add(data);\r\n                        }\r\n   " +
                    "                 }\r\n                }\r\n            }\r\n\r\n            return bookD" +
                    "ata;\r\n        }\r\n\r\n        /// <summary>Inserts the given ");
            
            #line 283 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" data model into the ");
            
            #line 283 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" table.</summary>\r\n        /// <param name=\"");
            
            #line 284 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RecordCamel));
            
            #line default
            #line hidden
            this.Write("Data\">The ");
            
            #line 284 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RecordLower));
            
            #line default
            #line hidden
            this.Write(" data to insert.</param>\r\n        /// <returns>The ID of the inserted ");
            
            #line 285 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" record.</returns>\r\n        public int Insert(");
            
            #line 286 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("DataModel ");
            
            #line 286 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RecordCamel));
            
            #line default
            #line hidden
            this.Write("Data)\r\n        {\r\n            if (");
            
            #line 288 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RecordCamel));
            
            #line default
            #line hidden
            this.Write("Data == null)\r\n            {\r\n                throw new ArgumentNullException(nam" +
                    "eof(");
            
            #line 290 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RecordCamel));
            
            #line default
            #line hidden
            this.Write(@"Data));
            }

            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                throw new InvalidOperationException(""Connection String is null.  Insert operation cannot be performed."");
            }

            if (");
            
            #line 298 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RecordCamel));
            
            #line default
            #line hidden
            this.Write("Data.");
            
            #line 298 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey));
            
            #line default
            #line hidden
            this.Write(" > 0)\r\n            {\r\n                throw new InvalidOperationException(\"Unable" +
                    " to insert ");
            
            #line 300 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" Data that already has a ");
            
            #line 300 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey));
            
            #line default
            #line hidden
            this.Write(".\");\r\n            }\r\n\r\n            var ");
            
            #line 303 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.ToCamelCase()));
            
            #line default
            #line hidden
            this.Write(" = -1;\r\n            using (var connection = new SqlConnection(this.ConnectionStri" +
                    "ng))\r\n            {\r\n                using (var command = new SqlCommand(\"");
            
            #line 306 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateInsertProcName()));
            
            #line default
            #line hidden
            this.Write("\"))\r\n                {\r\n                    command.CommandType = CommandType.Sto" +
                    "redProcedure;\r\n                    command.Connection = connection;\r\n           " +
                    "         ");
            
            #line 310 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateInsertProcParams()));
            
            #line default
            #line hidden
            this.Write(@"

                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ");
            
            #line 318 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.ToCamelCase()));
            
            #line default
            #line hidden
            this.Write(" = (int)reader[\"");
            
            #line 318 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey));
            
            #line default
            #line hidden
            this.Write("\"];\r\n                        }\r\n                    }\r\n                }\r\n       " +
                    "     }\r\n\r\n            return ");
            
            #line 324 "C:\git-scratch\vanilladb\VanillaDb\DataProviders\SqlDataProvider.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.ToCamelCase()));
            
            #line default
            #line hidden
            this.Write(@";
        }

        /// <summary>Parses the book data model out of the reader.</summary>
        /// <param name=""reader"">The reader.</param>
        /// <returns>Populated Book Data Model</returns>
        private BookDataModel ParseBookDataModel(IDataReader reader)
        {
            var data = new BookDataModel();
            data.BookId = (int)reader[""BookId""];
            data.ISBN10 = reader[""ISBN10""] != DBNull.Value ? (string)reader[""ISBN10""] : string.Empty;
            data.ISBN13 = reader[""ISBN13""] != DBNull.Value ? (string)reader[""ISBN13""] : string.Empty;
            data.ASIN10 = reader[""ASIN10""] != DBNull.Value ? (string)reader[""ASIN10""] : string.Empty;
            return data;
        }
    }
}
");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class SqlDataProviderBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}

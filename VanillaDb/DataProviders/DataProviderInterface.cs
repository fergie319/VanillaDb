﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace VanillaDb.DataProviders
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using VanillaDb;
    using VanillaDb.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DataProviderInterface : DataProviderInterfaceBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("// <copyright file=\"");
            
            #line 8 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateName()));
            
            #line default
            #line hidden
            this.Write(".cs\" company=\"");
            
            #line 8 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.Company));
            
            #line default
            #line hidden
            this.Write("\">\r\n// Copyright (c) ");
            
            #line 9 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.Company));
            
            #line default
            #line hidden
            this.Write(@". All rights reserved.
// </copyright>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace ");
            
            #line 19 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.Namespace));
            
            #line default
            #line hidden
            this.Write(".DataProviders.");
            
            #line 19 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.TableAlias));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    using System;\r\n    using System.Collections.Generic;\r\n    using System.T" +
                    "hreading.Tasks;\r\n\r\n    /// <summary>Interface for ");
            
            #line 25 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" Data Providers.</summary>\r\n    public partial interface ");
            
            #line 26 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateName()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n");
            
            #line 28 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    // Generate the Insert method if configured
    if (Table.Config.Insert)
    {

            
            #line default
            #line hidden
            this.Write("        ");
            
            #line 33 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateInsertMethod()));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n");
            
            #line 35 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    }

            
            #line default
            #line hidden
            
            #line 38 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    // Generate the Update method if configured
    if (Table.Config.Update)
    {

            
            #line default
            #line hidden
            this.Write("        ");
            
            #line 43 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GenerateUpdateMethod()));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n");
            
            #line 45 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    }

            
            #line default
            #line hidden
            
            #line 48 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    // Generate the Delete method if configured
    if (Table.Config.Delete)
    {

            
            #line default
            #line hidden
            this.Write("        /// <summary>Deletes record with the given ");
            
            #line 53 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.FieldName));
            
            #line default
            #line hidden
            this.Write(" from the ");
            
            #line 53 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.TableName));
            
            #line default
            #line hidden
            this.Write(" table.</summary>\r\n        /// <param name=\"");
            
            #line 54 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.GetCodeParamName()));
            
            #line default
            #line hidden
            this.Write("\">The ");
            
            #line 54 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.FieldName));
            
            #line default
            #line hidden
            this.Write(".</param>\r\n        Task<int> Delete(int ");
            
            #line 55 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.GetCodeParamName()));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n        /// <summary>Deletes the records with the given ");
            
            #line 57 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.FieldName));
            
            #line default
            #line hidden
            this.Write("s from the ");
            
            #line 57 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.TableName));
            
            #line default
            #line hidden
            this.Write(" table.</summary>\r\n        /// <param name=\"");
            
            #line 58 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.GetCodeParamName()));
            
            #line default
            #line hidden
            this.Write("s\">The ");
            
            #line 58 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.FieldName));
            
            #line default
            #line hidden
            this.Write("s.</param>\r\n        Task<int> Delete(IEnumerable<int> ");
            
            #line 59 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PrimaryKey.GetCodeParamName()));
            
            #line default
            #line hidden
            this.Write("s);\r\n\r\n");
            
            #line 61 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    }

            
            #line default
            #line hidden
            
            #line 64 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    // Generate the GetAll method if configured
    if (Table.Config.GetAll)
    {

            
            #line default
            #line hidden
            this.Write("        /// <summary>Gets the data for all ");
            
            #line 69 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("(s) from the ");
            
            #line 69 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.TableName));
            
            #line default
            #line hidden
            this.Write(" table.</summary>");
            
            #line 69 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAllParamsXmlComments()));
            
            #line default
            #line hidden
            this.Write("\r\n        /// <returns>Enumerable of ");
            
            #line 71 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" Data Model or null if not found.</returns>\r\n        Task<IEnumerable<");
            
            #line 72 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.GetDataModelName()));
            
            #line default
            #line hidden
            this.Write(">> GetAll(");
            
            #line 72 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetAllMethodParams()));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 74 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    }

            
            #line default
            #line hidden
            
            #line 77 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    // Generate the GetBy-Index and GetBy-Index-Bulk methods for each index
    foreach (IndexModel index in Indexes)
    {

            
            #line default
            #line hidden
            this.Write("        /// <summary>Gets the data for the ");
            
            #line 82 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" with the given ");
            
            #line 82 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.ReadableFields()));
            
            #line default
            #line hidden
            this.Write(".</summary>\r\n        ");
            
            #line 83 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.GetByIndexParamsXmlComments()));
            
            #line default
            #line hidden
            this.Write("\r\n        /// <returns>");
            
            #line 84 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" Data Model or null if not found</returns>\r\n        Task<");
            
            #line 85 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.GetByIndexReturnType()));
            
            #line default
            #line hidden
            this.Write("> ");
            
            #line 85 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.GetByIndexMethodName()));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 85 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.GetByIndexMethodParams()));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n        /// <summary>Gets the data for the ");
            
            #line 87 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write("(s) with the given collection of ");
            
            #line 87 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.ReadableFields()));
            
            #line default
            #line hidden
            this.Write(".</summary>\r\n        ");
            
            #line 88 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.BulkGetByIndexParamsXmlComments()));
            
            #line default
            #line hidden
            this.Write("\r\n        /// <returns>Enumerable of ");
            
            #line 89 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Record));
            
            #line default
            #line hidden
            this.Write(" Data Model or null if not found.</returns>\r\n        Task<IEnumerable<");
            
            #line 90 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Table.GetDataModelName()));
            
            #line default
            #line hidden
            this.Write(">> ");
            
            #line 90 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.GetByIndexMethodName()));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 90 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(index.BulkGetByIndexMethodParams()));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 92 "C:\git\VanillaDb\VanillaDb\DataProviders\DataProviderInterface.tt"

    }

            
            #line default
            #line hidden
            this.Write("    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public class DataProviderInterfaceBase
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
        public System.Text.StringBuilder GenerationEnvironment
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

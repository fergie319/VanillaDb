using System;
using System.Collections.Generic;
using VanillaDb.Models;

namespace VanillaDb
{
    /// <summary>Extension methods for System.Type.</summary>
    public static class TypeExtensions
    {
        // This is the set of types from the C# keyword list.
        private static Dictionary<Type, string> _typeAlias = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(void), "void" }
        };

        /// <summary>Gets the alias of a type (e.g. int instead of Int32) or the type name if it has no alias</summary>
        /// <param name="type">The type.</param>
        /// <returns>Type name or alias.</returns>
        public static string GetAliasOrName(this FieldTypeModel type)
        {
            if (!_typeAlias.TryGetValue(type.FieldType, out string alias))
            {
                alias = type.FieldType.Name;
            }

            var nullableChar = (type.IsNullable && type.FieldType != typeof(string)) ? "?" : string.Empty;
            alias += nullableChar;

            return alias;
        }

        /// <summary>Gets the default value for a type and returns it as a string.</summary>
        /// <param name="type">The type.</param>
        /// <returns>Type value as a string</returns>
        public static string GetDefaultValueString(this Type type)
        {
            var defaultValueString = "null";
            if (type == typeof(string))
            {
                defaultValueString = "string.Empty";
            }
            else if (type == typeof(int))
            {
                defaultValueString = "-1";
            }
            else if (type == typeof(bool))
            {
                defaultValueString = "false";
            }

            return defaultValueString;
        }
    }
}

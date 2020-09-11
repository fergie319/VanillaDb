namespace VanillaDb
{
    /// <summary>Extension methods that target the System.String class.</summary>
    public static class StringExtensions
    {
        /// <summary>Converts this string to Camel Casing - does not funciton the same as TextInfo.ToTitleCase does with whitespace..</summary>
        /// <param name="thisString">The string to convert.</param>
        /// <returns>Camel-cased string</returns>
        public static string ToCamelCase(this string thisString)
        {
            var camelCaseString = thisString;
            if (!string.IsNullOrEmpty(thisString))
            {
                // If the string is the same after converting to upper, then assume it's an acronym and should be all lowercase
                if (thisString.ToUpper() == thisString)
                {
                    camelCaseString = thisString.ToLower();
                }
                else
                {
                    camelCaseString = char.ToLowerInvariant(thisString[0]) + thisString.Substring(1);
                }
            }

            return camelCaseString;
        }
    }
}

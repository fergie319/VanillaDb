namespace VanillaDb
{
    /// <summary>Interface for generating code - or any transformed text - automatically.</summary>
    public interface ICodeTemplate
    {
        /// <summary>Transforms the T4 template to generate structured code/sql/whatever.</summary>
        /// <returns>Transformed result string.</returns>
        string TransformText();

        /// <summary>Generates the name for the generated file.</summary>
        /// <returns>Extensionless file name.</returns>
        string GenerateName();
    }
}

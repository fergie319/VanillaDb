using System.IO;

namespace VanillaDb
{
    /// <summary>Extension methods for the ICodeTemplate interface.</summary>
    public static class CodeTemplateExtensions
    {
        /// <summary>Generates the file from the given template and saves it to the given folder.</summary>
        /// <param name="template">The template generator.</param>
        /// <param name="folderPath">The folder path.</param>
        public static void GenerateFile(this ICodeTemplate template, string folderPath)
        {
            var content = template.TransformText();
            Program.LogVerbose($"Content: {0}", content);
            File.WriteAllText($"{folderPath}\\{template.GenerateName()}.cs", content);
        }
    }
}

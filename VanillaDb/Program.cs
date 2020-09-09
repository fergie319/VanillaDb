using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaDb
{
    /// <summary>Main Program Class</summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("<Table>.sql file must be supplied as the first argument.");
            }

            // First param is the table file - expect *.sql
            var sqlFileName = args[0];
            var sqlFileInfo = new FileInfo(sqlFileName);
            if (!sqlFileInfo.Exists)
            {
                throw new InvalidOperationException($"{sqlFileName} does not exist.");
            }

            // Open the file and start parsing
            using (var stream = sqlFileInfo.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                // Read the file one line at a time until the end of the statement to build the list of fields
                var currentLine = reader.ReadLine();
                while (currentLine != "GO;")
                {
                    // blah blah blah
                }

                // Read any Indexes by skipping to the ON clause - write warnings about anything that doesn't parse
            }

            return 0;
        }
    }
}

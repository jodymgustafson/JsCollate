using System;
using System.Collections.Generic;
using System.IO;

namespace JsCollate
{
    public class JsCollator
    {
        /// <summary>
        /// Collates the script tags from the source to the destination folder
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <param name="destFolder"></param>
        /// <param name="header"></param>
        /// <param name="compress"></param>
        public static void Collate(string htmlFile, string destFolder, string header, bool compress, bool addTimestamp)
        {
            var destFiles = ScriptCollator.Collate(htmlFile, destFolder, addTimestamp);

            if (compress)
            {
                if (ScriptCompressor.CompressFiles(destFiles, destFolder))
                {
                    Console.WriteLine("Compressed successfully.");
                }
                else
                {
                    WriteFiles(destFiles, destFolder, header);
                    Console.WriteLine("Failed.");
                    throw new ApplicationException("Compression failed. See error messages");
                }
            }
            else
            {
                Console.WriteLine("Compression turned off.");
            }

            WriteFiles(destFiles, destFolder, header);
            Console.WriteLine("Completed successfully.");
        }

        /// <summary>
        /// Writes out to the collated file(s) to the destination folder
        /// </summary>
        /// <param name="destFiles"></param>
        /// <param name="destFolder"></param>
        /// <param name="header"></param>
        private static void WriteFiles(IEnumerable<CollatedScript> destFiles, string destFolder, string header)
        {
            foreach (var file in destFiles)
            {
                // Write to dest file
                var fileName = Path.Combine(destFolder, file.FileName);
                Console.WriteLine("Writing collated file: " + fileName);
                File.WriteAllText(fileName, GetComment(header) + Environment.NewLine + file.FileContents);
            }
        }

        /// <summary>
        /// Wraps a string in a comment if not already commented
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string GetComment(string text)
        {
            if (text.StartsWith("/*") || text.StartsWith("//")) return text;

            return "/* " + text + " */";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsCollate
{
    class HtmlScriptCollator
    {
        /// <summary>
        /// Collates script tags that have a data-collate attribute and writes the destination HTML file
        /// </summary>
        /// <returns>A list of script files to collate</returns>
        public static IEnumerable<FileToCollate> Collate(string htmlFile, string destFolder)
        {
            IEnumerable<FileToCollate> list;

            Console.WriteLine("Reading from: " + htmlFile);
            using (StreamReader reader = new StreamReader(htmlFile))
            {
                string destHtmlFile = Path.Combine(destFolder, Path.GetFileName(htmlFile));
                Console.WriteLine("Writing to: " + destHtmlFile);
                using (var writer = new StreamWriter(destHtmlFile, false))
                {
                    list = ProcessScriptTags(reader, writer);
                }
            }

            return list;
        }

        /// <summary>
        /// Looks through the HTML file for script tags that have data-collate attribute, removes them and adds them
        /// to the list of files to collate.
        /// </summary>
        /// <param name="reader">The HTML source file</param>
        /// <param name="writer">The HTML destination file</param>
        /// <returns>List of script files to collate</returns>
        private static IEnumerable<FileToCollate> ProcessScriptTags(StreamReader reader, StreamWriter writer)
        {
            List<FileToCollate> list = new List<FileToCollate>();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!IsScriptTag(line))
                {
                    writer.WriteLine(line);
                }
                else
                {
                    var destFile = GetCollateTo(line);
                    FileToCollate ftc = list.FirstOrDefault(x => x.DestFile == destFile);
                    if (ftc == null)
                    {
                        // Doesn't exist yet
                        ftc = new FileToCollate()
                        {
                            SourceFiles = new List<string>(),
                            DestFile = destFile
                        };
                        list.Add(ftc);
                    }

                    var sourceFile = GetSourceFile(line);
                    ((List<string>)ftc.SourceFiles).Add(sourceFile);

                    if (sourceFile == ftc.DestFile)
                    {
                        // If the file is being collated to itself we need to keep it
                        writer.WriteLine(line);
                    }
                    else
                    {
                        Console.WriteLine("Removing script: " + line.Trim());
                    }
                }
            }

            return list;
        }

        private static Regex reCollateTo = new Regex(@"<script\s+.*data-collate=.*>");

        private static bool IsScriptTag(string line)
        {
            return reCollateTo.IsMatch(line);
        }

        /// <summary>
        /// Gets the value of the src attribute
        /// </summary>
        private static string GetSourceFile(string line)
        {
            return GetAttributeValue("src", line);
        }

        /// <summary>
        /// Gets the value of the data-collate attribute
        /// </summary>
        private static string GetCollateTo(string line)
        {
            var collateTo = GetAttributeValue("data-collate", line);
            return collateTo;
        }

        private static string GetAttributeValue(string attrName, string line)
        {
            int idx = line.IndexOf(attrName + "=") + (attrName + "=").Length;
            if (idx > 0)
            {
                // line[idx] will be a double or single quote, find the ending quote
                int end = line.IndexOf(line[idx], idx + 1) - 1;
                string fileName = line.Substring(idx + 1, end - idx);
                return fileName;
            }
            return null;
        }
    }
}

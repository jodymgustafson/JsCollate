using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JsCollate
{
    class HtmlScriptReplacer
    {
        /// <summary>
        /// Replaces script tags inside replace:with groupings and writes the destination HTML file
        /// </summary>
        /// <returns>A list of script files to collate</returns>
        public static IEnumerable<FileToCollate> Replace(string htmlFile, string destFolder, bool addTiemstamp)
        {
            IEnumerable<FileToCollate> list;

            Console.WriteLine("Reading from: " + htmlFile);
            using (StreamReader reader = new StreamReader(htmlFile))
            {
                string destHtmlFile = Path.Combine(destFolder, Path.GetFileName(htmlFile));
                Console.WriteLine("Writing to: " + destHtmlFile);
                using (var writer = new StreamWriter(destHtmlFile, false))
                {
                    list = ProcessScriptTags(reader, writer, addTiemstamp);
                }
            }

            return list;
        }

        /// <summary>
        /// Looks through the HTML file for script tags that are inside a replace:with grouping,
        /// removes them, and adds them to the list of files to collate.
        /// </summary>
        /// <param name="reader">The HTML source file</param>
        /// <param name="writer">The HTML destination file</param>
        /// <returns>List of script files to collate</returns>
        private static IEnumerable<FileToCollate> ProcessScriptTags(StreamReader reader, StreamWriter writer, bool addTiemstamp)
        {
            List<FileToCollate> list = new List<FileToCollate>();

            bool isInReplace = false;
            FileToCollate ftc = null;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (IsBeginReplace(line))
                {
                    isInReplace = true;
                    ftc = new FileToCollate()
                    {
                        SourceFiles = new List<string>(),
                        DestFile = GetReplaceWith(line)
                    };
                    list.Add(ftc);
                    var ts = addTiemstamp ? "?v=" + DateTime.Now.Ticks : "";
                    writer.WriteLine(string.Format("<script src='{0}{1}'></script>", ftc.DestFile, ts));
                    continue;
                }

                if (!isInReplace)
                {
                    // Let it go through
                    writer.WriteLine(line);
                }
                else
                {
                    if (IsEndReplace(line))
                    {
                        isInReplace = false;
                    }
                    else
                    {
                        // Add script to the list of files to replace
                        var sourceFile = GetSourceFile(line);
                        ((List<string>)ftc.SourceFiles).Add(sourceFile);
                        Console.WriteLine("Removing script: " + line.Trim());
                    }
                }
            }

            return list;
        }

        private static Regex reReplaceWith = new Regex(@"<!--\s*replace:with(.*)\s*-->");
        private static bool IsBeginReplace(string line)
        {
            return reReplaceWith.IsMatch(line);
        }
        
        private static Regex reEndReplace = new Regex(@"<!--\s*endreplace\s*-->");
        private static bool IsEndReplace(string line)
        {
            return reEndReplace.IsMatch(line);
        }

        /// <summary>
        /// Gets the value of the src attribute
        /// </summary>
        private static string GetSourceFile(string line)
        {
            return GetAttributeValue("src", line);
        }

        /// <summary>
        /// Gets the value of the replace with
        /// </summary>
        private static string GetReplaceWith(string line)
        {
            var idx = line.IndexOf("replace:with(") + "replace:with(".Length;
            var end = line.IndexOf(")", idx);
            return line.Substring(idx, end - idx).Trim();
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

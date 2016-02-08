using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JsCollate
{
    public class ScriptCollator
    {
        /// <summary>
        /// Collates the contents of the script files referenced by the HTML file into one or more destination files
        /// </summary>
        /// <param name="htmlFile"></param>
        /// <param name="destFolder"></param>
        /// <returns></returns>
        public static IEnumerable<CollatedScript> Collate(string htmlFile, string destFolder, bool addTiemstamp)
        {
            // check if they're using the older data-collate attribute
            IEnumerable<FileToCollate> filesToCollate = HtmlScriptCollator.Collate(htmlFile, destFolder);
            if (filesToCollate.Count() == 0)
            {
                // they must be using replace:with grouping
                filesToCollate = HtmlScriptReplacer.Replace(htmlFile, destFolder, addTiemstamp);
            }

            string sourceDir = Path.GetDirectoryName(htmlFile);
            IEnumerable<CollatedScript> destFiles = CollateFiles(filesToCollate, sourceDir);
            return destFiles;
        }

        /// <summary>
        /// Collates the files into scripts
        /// </summary>
        /// <param name="filesToCollate"></param>
        /// <param name="sourceDir"></param>
        /// <returns></returns>
        private static IEnumerable<CollatedScript> CollateFiles(IEnumerable<FileToCollate> filesToCollate, string sourceDir)
        {
            var collatedScripts = new List<CollatedScript>();

            foreach (var fileToCollate in filesToCollate)
            {
                CollatedScript collatedScript = CollateScripts(sourceDir, fileToCollate);
                collatedScripts.Add(collatedScript);
            }

            return collatedScripts;
        }

        private static CollatedScript CollateScripts(string sourceDir, FileToCollate fileToCollate)
        {
            var srcFilePaths = GetSourceFilePaths(fileToCollate.SourceFiles, sourceDir);
            return new CollatedScript()
                {
                    FileName = fileToCollate.DestFile,
                    FileContents = CombineFileContents(srcFilePaths)
                };
        }

        private static IEnumerable<string> GetSourceFilePaths(IEnumerable<string> filesPaths, string sourceDir)
        {
            return filesPaths.Select(path => Path.Combine(sourceDir, path));
        }

        /// <summary>
        /// Reads the contents of all the files and combines them into one string
        /// </summary>
        /// <param name="sourceFilePaths">List of files to combine</param>
        /// <returns></returns>
        private static string CombineFileContents(IEnumerable<string> sourceFilePaths)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var sourceFile in sourceFilePaths)
            {
                // Add it to the collated text
                sb.AppendLine(File.ReadAllText(sourceFile));
            }

            return sb.ToString();
        }
    }
}

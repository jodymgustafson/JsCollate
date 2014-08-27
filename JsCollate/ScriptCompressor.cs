using System;
using System.Collections.Generic;
using EcmaScript.NET;
using Yahoo.Yui.Compressor;

namespace JsCollate
{
    public class ScriptCompressor
    {
        /// <summary>
        /// Compresses the collated file(s) and writes them out to the destination folder
        /// </summary>
        /// <param name="destFiles"></param>
        /// <param name="destFolder"></param>
        /// <returns>True if successfully compressed</returns>
        public static bool CompressFiles(IEnumerable<CollatedScript> destFiles, string destFolder)
        {
            var jsCompressor = new JavaScriptCompressor();
            foreach (var file in destFiles)
            {
                Console.WriteLine("Compressing: " + file.FileName);
                try
                {
                    file.FileContents = jsCompressor.Compress(file.FileContents);
                }
                catch (EcmaScriptRuntimeException ex)
                {
                    Console.WriteLine(file.FileName + ":" + ex.LineNumber + "," + ex.ColumnNumber + ": ERROR - " + ex.Message);
                    Console.WriteLine(ex.LineSource);
                    return false;
                }
            }
            return true;
        }
    }
}

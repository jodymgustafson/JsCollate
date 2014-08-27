using System;
using System.Linq;

namespace JsCollate
{
    /// <summary>
    /// JsCollate collates JavaScript files referenced in an HTML file into one or more files,
    /// compresses the JavaScript code,
    /// then updates the script tags in the HTML file to the output file(s).
    /// 
    /// Specify the file to collate to by adding a data-collate parameter to your script elements.
    /// The following would collate the two files to app.js.
    /// NOTE: Each script element must be on a separate line.
    /// <example>
    /// &lt;script src="myFile.js" data-collate="app.js"&gt;&lt;/script&gt;
    /// &lt;script src="myOtherFile.js" data-collate="app.js"&gt;&lt;/script&gt;
    /// &lt;script src="app.js" data-collate="app.js"&gt;&lt;/script&gt;
    /// </example>
    /// 
    /// Usage: JsCollate source dest [/header:text] [/-c]
    /// Command line params:
    /// source - The HTML file that contains script tags to collate
    /// dest - The folder to put the updated HTML and JavaScript files in
    /// /header:text - (optional) Places the specified text at the beginning of the output JavaScript file as a comment
    /// /-c - (optional) Do not compress the JavaScript
    /// 
    /// The following example collates the JavaScript files referenced in app.html to the release folder.
    /// <example>JsCollate app.html release /header:"My App version 1.0"</example>
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Any(a => a == "/?"))
            {
                ShowHelp();
                return;
            }

            string sourceHtml = args[0];
            string destFolder = args[1];
            string header = "";
            bool compress = true;

            for (var i = 2; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("/header:", StringComparison.InvariantCulture))
                {
                    header = arg.Substring("/header:".Length).Trim();
                }
                else if (arg.ToLower() == "/-c")
                {
                    compress = false;
                }
                else
                {
                    throw new ApplicationException("Invalid argument: " + arg);
                }
            }

            JsCollator.Collate(sourceHtml, destFolder, header, compress);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Collates files referenced by script tags in an HMTL file to a singe file");
            Console.WriteLine("JsCollate source dest [/header:text] [/-c]");
            Console.WriteLine("source       : Source HTML file");
            Console.WriteLine("dest         : Destination folder");
            Console.WriteLine("/header:text : Text that will be inserted at the beginning of the collated script file");
            Console.WriteLine("/-c          : Don't compress JavaScript");
        }
    }
}

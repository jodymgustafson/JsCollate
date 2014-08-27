using System.Collections.Generic;

namespace JsCollate
{
    class FileToCollate
    {
        public IEnumerable<string> SourceFiles { get; set; }
        public string DestFile { get; set; }
    }
}

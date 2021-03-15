using System.Collections.Generic;
using System.IO;

namespace SimpleTemplateRenderer
{
    public class FileRenderArgs
    {
        public FileInfo TemplateFile { get; set; }
        public FileInfo OutputFile { get; set; }
        public Dictionary<string, string> TemplateVariables { get; set; }
        public bool TransformFilePath { get; set; }
    }
}
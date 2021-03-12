using System.Collections.Generic;
using System.IO;

namespace KGar.TemplateProjectGenerator
{
    internal class FileRenderArgs
    {
        public FileInfo TemplateFile { get; set; }
        public FileInfo OutputFile { get; set; }
        public Dictionary<string, string> TemplateVariables { get; internal set; }
    }
}
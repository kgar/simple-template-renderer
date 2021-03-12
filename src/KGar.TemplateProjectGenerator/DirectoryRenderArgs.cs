using System.Collections.Generic;
using System.IO;

namespace KGar.TemplateProjectGenerator
{
    internal class DirectoryRenderArgs
    {
        public DirectoryInfo TemplateDirectory { get; set; }
        public DirectoryInfo OutputDirectory { get; set; }
        public FileInfo Gitignore { get; set; }
        public Dictionary<string, string> TemplateVariables { get; internal set; }
    }
}
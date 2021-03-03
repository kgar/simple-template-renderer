using System.Collections.Generic;
using System.IO;

namespace KGar.TemplateProjectGenerator
{
    internal class Args
    {
        public string TemplatePath { get; internal set; }
        public DirectoryInfo OutputDirectory { get; internal set; }
        public Dictionary<string, string> TemplateVariables { get; internal set; }
        public FileInfo? Gitignore { get; internal set; }
    }
}
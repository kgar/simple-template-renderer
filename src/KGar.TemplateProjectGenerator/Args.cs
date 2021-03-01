using System.Collections.Generic;

namespace KGar.TemplateProjectGenerator
{
    internal class Args
    {
        public string TemplateDirectory { get; set; }
        public string OutputDirectory { get; set; }
        public Dictionary<string, string> TemplateVariables { get; set; }
    }
}
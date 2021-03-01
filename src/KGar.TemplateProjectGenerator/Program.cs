using System;

namespace KGar.TemplateProjectGenerator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // dotnet template-project-generator <TEMPLATEPROJECTPATH> <GENERATEDPROJECTPATH> --template-vars-dir <PATH>
            // dotnet template-project-generator "./TemplateProject/TemplateProject.csproj" "./GeneratedProject/GeneratedProject.csproj" --template-vars-dir "./settings.json" 
            foreach (var arg in args)
            {
                System.Console.WriteLine(arg);
            }
        }
    }
}

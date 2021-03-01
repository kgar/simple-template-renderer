using System;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace KGar.TemplateProjectGenerator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // <TEMPLATEPROJECTPATH> Path to the CSPROJ file 
            // <OUTPUTPATH> Path to the generated CSPROJ file
            // <TEMPLATEVARIABLESJSONPATH> Optional path to the JSON file with key-value-pairs to use when transforming the template 
            // dotnet template-project-generator "./TemplateProject/TemplateProject.csproj" "./GeneratedProject/GeneratedProject.csproj" --template-vars-dir "./settings.json" 
            if (args.Length < 3)
            {
                var sb = new StringBuilder();
                sb.Append("Could not execute because the required args were not provided. Args provided: ")
                    .Append(args.Length)
                    .AppendLine(". Args required: 3");
                sb.AppendLine("  dotnet tool format:");
                sb.AppendLine("    dotnet kgar-template-project-generator <TEMPLATEPROJECTPATH> <GENERATEDPROJECTPATH> <TEMPLATEVARIABLESJSONPATH>");
                sb.AppendLine("  local build dotnet run format:");
                sb.AppendLine("    dotnet run <TEMPLATEPROJECTPATH> <OUTPUTPATH> <TEMPLATEVARIABLESJSONPATH>");
                WriteError(sb.ToString());
                return;
            }

            var templateProjectPath = args[0];

            if (!File.Exists(templateProjectPath) && !Directory.Exists(templateProjectPath))
            {
                WriteError("Template Project path/file not found.");
                return;
            }
            // var templatePathIsProject = templateProjectPath.EndsWith(".csproj", true, System.Globalization.CultureInfo.CurrentCulture);
            var templateProjectDir = Path.GetDirectoryName(templateProjectPath);
            var files = Directory.GetFiles(templateProjectDir, "*", SearchOption.AllDirectories);

            var outputPath = args[1];
            try
            {
                var outputPathDirectory = Path.GetDirectoryName(outputPath);
                Directory.CreateDirectory(outputPathDirectory);
            }
            catch
            {
                WriteError("Unable to execute. Output path is not valid.");
                return;
            }

            var jsonPath = args[2];
            if (!File.Exists(jsonPath))
            {
                WriteError("Unable to execute. Template Variables JSON Path <TEMPLATEVARIABLESJSONPATH> file not found.");
                return;
            }

            var jsonData = JsonSerializer
                .Deserialize<Dictionary<string, string>>(
                    File.ReadAllText(jsonPath));

            foreach (var key in jsonData.Keys)
            {
                System.Console.WriteLine($"Key: {key} | Value: {jsonData[key]}");
            }
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

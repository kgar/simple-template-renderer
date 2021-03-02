using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

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

            var (isValid, parsedArgs) = AssertValid(args);

            if (!isValid)
            {
                return;
            }

            GenerateFromTemplate(parsedArgs);
        }

        private static (bool isValid, Args parsedArgs) AssertValid(string[] args)
        {
            var templatePath = args[0];

            if (!File.Exists(templatePath) && !Directory.Exists(templatePath))
            {
                WriteError("Template Project path/file not found.");
                return (false, null);
            }

            var templateProjectDir = File.Exists(templatePath)
                ? Path.GetDirectoryName(templatePath)
                : templatePath;

            var outputPath = args[1];
            try
            {
                var outputPathDirectory = Path.GetDirectoryName(outputPath);
                Directory.CreateDirectory(outputPathDirectory);
            }
            catch
            {
                WriteError("Unable to execute. Output path is not valid.");
                return (false, null);
            }

            var jsonPath = args[2];
            if (!File.Exists(jsonPath))
            {
                WriteError("Unable to execute. Template Variables JSON Path <TEMPLATEVARIABLESJSONPATH> file not found.");
                return (false, null);
            }

            Dictionary<string, string> jsonData;

            try
            {
                jsonData = JsonSerializer
                .Deserialize<Dictionary<string, string>>(
                    File.ReadAllText(jsonPath));
            }
            catch
            {
                WriteError("Unable to execute. Valid template variables JSON cannot be found.");
                return (false, null);
            }

            var parsedArgs = new Args
            {
                OutputDirectory = outputPath,
                TemplateDirectory = templateProjectDir,
                TemplateVariables = jsonData
            };

            return (true, parsedArgs);
        }

        private static void GenerateFromTemplate(Args args)
        {
            var files = Directory.GetFiles(args.TemplateDirectory, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                Console.WriteLine($"source: {file}");
                Console.WriteLine($"original contents: {text}");

                var target = file.Replace(args.TemplateDirectory, args.OutputDirectory);
                Console.WriteLine($"target: {target}");

                var transformedTarget = TransformText(target, args.TemplateVariables);
                Console.WriteLine($"transformed target: {transformedTarget}");

                var transformedText = TransformText(text, args.TemplateVariables);
                Console.WriteLine($"transformed contents: {transformedText}");
            }
        }

        private static string TransformText(string text, Dictionary<string, string> variables)
        {
            // TODO: See if string replace can take an array of criteria
            foreach (var key in variables.Keys)
            {
                var token = GetTemplateToken(key);
                var replacementValue = variables[key];
                text = text.Replace(token, replacementValue);
            }

            return text;
        }

        private static string GetTemplateToken(string variableName)
        {
            return $"__{variableName}__";
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

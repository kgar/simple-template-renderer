using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KGar.TemplateProjectGenerator
{
    internal static class Program
    {
        internal static Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Argument<string>(
                    "--templatePath",
                    "The path to the template directory or file."),
                new Argument<DirectoryInfo>(
                    "--outputPath",
                    "The path to the destination directory or file."),
                new Argument<FileInfo>(
                    "--templateVariablesPath",
                    "The path to the JSON template variables object."),
                new Option<FileInfo> (
                    "--gitignore",
                    "Optional path to a gitignore file to use when determining what to copy.")
            };
            rootCommand.Description = "TODO: Describe meeeeeee";

            rootCommand.Handler = CommandHandler.Create<string, DirectoryInfo, FileInfo, FileInfo>((
                templatePath,
                outputPath,
                templateVariablesPath,
                gitignore) =>
            {
                var templateVariables = JsonSerializer
                    .Deserialize<Dictionary<string, string>>(
                        File.ReadAllText(templateVariablesPath.FullName));

                var args = new Args
                {
                    TemplatePath = templatePath,
                    OutputDirectory = outputPath,
                    TemplateVariables = templateVariables,
                    Gitignore = gitignore
                };

                GenerateFromTemplate(args);
            });

            return rootCommand.InvokeAsync(args);
        }

        private static void GenerateFromTemplate(Args args)
        {
            var files = Directory.GetFiles(args.TemplatePath, "*", SearchOption.AllDirectories);
            var ignore = new Ignore.Ignore();
            if (args.Gitignore.Exists)
            {
                ignore.Add(File.ReadAllLines(args.Gitignore.FullName));
            }

            foreach (var file in files)
            {
                if (ignore.IsIgnored(file))
                {
                    continue;
                }

                var text = File.ReadAllText(file);
                Console.WriteLine($"source: {file}");
                Console.WriteLine($"original contents: {text}");

                var templateDirectory = Directory.Exists(args.TemplatePath)
                    ? args.TemplatePath
                    : Path.GetDirectoryName(args.TemplatePath);

                var targetSubPath = file.Replace(templateDirectory, string.Empty);
                var target = Path.Combine(args.OutputDirectory.FullName, targetSubPath);
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

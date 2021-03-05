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
            var directoryCommand = new Command("dir", "Recursively render template content starting in a specified directory.")
            {
                new Argument<DirectoryInfo>("--template", "The directory of the template content."),
                new Argument<DirectoryInfo>("--output", "The directory where the rendered content should be placed."),
                new Argument<FileInfo>("--variables", "The path to a JSON file with a flat object of template variables and their values."),
                new Option<FileInfo>("--gitignore", "The path to an optional gitignore file to filter out content when performing the operation."),
            };
            directoryCommand.Handler = CommandHandler.Create<DirectoryInfo, DirectoryInfo, FileInfo, FileInfo>(RenderTemplateDirectory);

            var fileCommand = new Command("file", "Render a template file.")
            {
                new Argument<FileInfo>("--template", "The directory of the template content."),
                new Argument<FileInfo>("--output", "The directory where the rendered content should be placed."),
                new Argument<FileInfo>("--variables", "The path to a JSON file with a flat object of template variables and their values."),
            };
            fileCommand.Handler = CommandHandler.Create<FileInfo, FileInfo, FileInfo>(RenderTemplateFile);

            var rootCommand = new RootCommand
            {
                directoryCommand,
                fileCommand
            };

            rootCommand.Description = "TODO: Describe meeeeeee";

            return rootCommand.InvokeAsync(args);
        }

        private static void RenderTemplateFile(FileInfo template, FileInfo output, FileInfo variables)
        {
            // TODO: Figure out how the CommandLine models can handle this parsing step...
            var templateVariables = JsonSerializer
                   .Deserialize<Dictionary<string, string>>(
                       File.ReadAllText(variables.FullName));
            
            Console.WriteLine("TODO: Implement RenderTemplateFile");
        }

        private static void RenderTemplateDirectory(DirectoryInfo template, DirectoryInfo output, FileInfo variables, FileInfo gitignore)
        {
            // TODO: Figure out how the CommandLine models can handle this parsing step...
            var templateVariables = JsonSerializer
                   .Deserialize<Dictionary<string, string>>(
                       File.ReadAllText(variables.FullName));

            Console.WriteLine("TODO: Implement RenderTemplateDirectory");
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

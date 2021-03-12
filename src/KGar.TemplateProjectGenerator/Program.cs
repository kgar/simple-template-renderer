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
                new Argument<DirectoryInfo>("template", "The directory of the template content."),
                new Argument<DirectoryInfo>("output", "The directory where the rendered content should be placed."),
                new Argument<FileInfo>("variables", "The path to a JSON file with a flat object of template variables and their values."),
                new Option<FileInfo>("--gitignore", "The path to an optional gitignore file to filter out content when performing the operation."),
            };
            directoryCommand.Handler = CommandHandler.Create<DirectoryInfo, DirectoryInfo, FileInfo, FileInfo>(RenderTemplateDirectory);

            var fileCommand = new Command("file", "Render a template file.")
            {
                new Argument<FileInfo>("template", "The file path to the template file."),
                new Argument<FileInfo>("output", "The file path where the rendered content should be placed."),
                new Argument<FileInfo>("variables", "The path to a JSON file with a flat object of template variables and their values."),
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

            var args = new FileRenderArgs
            {
                TemplateFile = template,
                OutputFile = output,
                TemplateVariables = templateVariables
            };

            RenderFileFromTemplate(args);
        }

        private static void RenderTemplateDirectory(DirectoryInfo template, DirectoryInfo output, FileInfo variables, FileInfo gitignore)
        {
            // TODO: Figure out how the CommandLine models can handle this parsing step...
            var templateVariables = JsonSerializer
                   .Deserialize<Dictionary<string, string>>(
                       File.ReadAllText(variables.FullName));

            Console.WriteLine("TODO: Implement RenderTemplateDirectory");
        }

        // private static void GenerateFromTemplate(FileRenderArgs args)
        // {
        //     var files = Directory.GetFiles(args.TemplatePath, "*", SearchOption.AllDirectories);
        //     var ignore = new Ignore.Ignore();
        //     if (args.Gitignore.Exists)
        //     {
        //         ignore.Add(File.ReadAllLines(args.Gitignore.FullName));
        //     }

        //     foreach (var file in files)
        //     {
        //         if (ignore.IsIgnored(file))
        //         {
        //             continue;
        //         }

        //         RenderFileFromTemplate(args, file);
        //     }
        // }

        private static void RenderFileFromTemplate(FileRenderArgs args)
        {
            var text = File.ReadAllText(args.TemplateFile.FullName);
            var transformedTargetFile = new FileInfo(TransformText(args.OutputFile.FullName, args.TemplateVariables));
            var transformedText = TransformText(text, args.TemplateVariables);
            transformedTargetFile.Directory.Create();
            File.WriteAllText(transformedTargetFile.FullName, transformedText);
        }

        private static string TransformText(string text, Dictionary<string, string> variables)
        {
            foreach (var kvp in variables)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return text;
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

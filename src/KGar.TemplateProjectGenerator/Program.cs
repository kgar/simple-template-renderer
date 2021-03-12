using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
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

            var args = new DirectoryRenderArgs
            {
                TemplateDirectory = template,
                OutputDirectory = output,
                TemplateVariables = templateVariables,
                Gitignore = gitignore
            };

            RenderDirectoryFromTemplate(args);
        }

        private static void RenderDirectoryFromTemplate(DirectoryRenderArgs args)
        {
            var files = Directory.GetFiles(args.TemplateDirectory.FullName, "*", SearchOption.AllDirectories);

            var ignore = new Ignore.Ignore();
            bool ignoreFiles = args.Gitignore?.Exists == true;
            if (ignoreFiles)
            {
                var patterns = File
                    .ReadAllLines(args.Gitignore.FullName)
                    .Where(x =>
                        !x.Trim().StartsWith("#") &&
                        !string.IsNullOrWhiteSpace(x));
                ignore.Add(patterns);
            }

            foreach (var templateFile in files)
            {
                var templateFileInfo = new FileInfo(templateFile);
                var templateFileRelativePath = templateFileInfo.FullName.Replace(args.TemplateDirectory.FullName, string.Empty).TrimStart('\\');

                if (ignoreFiles && ignore.IsIgnored(UseLinuxStyleRelativePath(templateFileRelativePath)))
                {
                    continue;
                }

                var outputFileInfo = new FileInfo(
                    Path.Combine(
                        args.OutputDirectory.FullName,
                        templateFileRelativePath));

                var fileRenderArgs = new FileRenderArgs
                {
                    TemplateFile = templateFileInfo,
                    OutputFile = outputFileInfo,
                    TemplateVariables = args.TemplateVariables
                };

                RenderFileFromTemplate(fileRenderArgs);
            }

            static string UseLinuxStyleRelativePath(string templateFileRelativePath)
            {
                return templateFileRelativePath.Replace('\\', '/');
            }
        }

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
    }
}

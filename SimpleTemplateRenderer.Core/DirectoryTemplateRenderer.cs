using System.IO;
using System.Linq;

namespace SimpleTemplateRenderer
{
    public static class DirectoryTemplateRenderer
    {
        public static void Render(DirectoryRenderArgs args)
        {
            var files = Directory.GetFiles(args.TemplateDirectory.FullName, "*", SearchOption.AllDirectories);

            var ignore = new Ignore.Ignore();
            var ignoreFiles = args.Gitignore?.Exists == true;
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
                    TemplateVariables = args.TemplateVariables,
                    TransformFilePath = args.TransformFilePath
                };

                FileTemplateRenderer.RenderFileFromTemplate(fileRenderArgs);
            }

            static string UseLinuxStyleRelativePath(string templateFileRelativePath)
            {
                return templateFileRelativePath.Replace('\\', '/');
            }
        }
    }
}
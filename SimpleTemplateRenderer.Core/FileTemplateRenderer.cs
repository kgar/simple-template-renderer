using System.IO;

namespace SimpleTemplateRenderer
{
    public static class FileTemplateRenderer
    {
        public static void RenderFileFromTemplate(FileRenderArgs args)
        {
            var targetFile = args.TransformFilePath
                ? new FileInfo(FindReplaceTextTransformer.Transform(args.OutputFile.FullName, args.TemplateVariables))
                : args.OutputFile;

            var text = File.ReadAllText(args.TemplateFile.FullName);
            var transformedText = FindReplaceTextTransformer.Transform(text, args.TemplateVariables);

            targetFile.Directory.Create();

            File.WriteAllText(targetFile.FullName, transformedText);
        }
    }
}
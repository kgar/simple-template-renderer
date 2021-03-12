using System.IO;

namespace SimpleTemplateRenderer
{
    public static class FileTemplateRenderer
    {
        public static void RenderFileFromTemplate(FileRenderArgs args)
        {
            var transformedTargetFile = new FileInfo(FindReplaceTextTransformer.Transform(args.OutputFile.FullName, args.TemplateVariables));

            var text = File.ReadAllText(args.TemplateFile.FullName);
            var transformedText = FindReplaceTextTransformer.Transform(text, args.TemplateVariables);

            transformedTargetFile.Directory.Create();

            File.WriteAllText(transformedTargetFile.FullName, transformedText);
        }
    }
}
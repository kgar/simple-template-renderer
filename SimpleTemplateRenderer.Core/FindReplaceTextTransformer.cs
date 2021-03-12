using System.Collections.Generic;

namespace SimpleTemplateRenderer
{
    public static class FindReplaceTextTransformer
    {
        public static string Transform(string text, Dictionary<string, string> variables)
        {
            foreach (var kvp in variables)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return text;
        }
    }
}
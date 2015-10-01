using Windows.UI.Xaml.Documents;
using HtmlAgilityPack;

namespace Gitter.HtmlToXaml.Helpers
{
    internal static class ParagraphHelper
    {
        public static void AddChildren(this Paragraph p, HtmlNode node, bool isCodeBlock = false)
        {
            if (isCodeBlock)
                p.Inlines.AddChildren(node, NodeHelper.GenerateCodeBlockForNode);
            else
                p.Inlines.AddChildren(node, NodeHelper.GenerateBlockForNode);
        }
    }
}

using Windows.UI.Xaml.Documents;
using HtmlAgilityPack;

namespace Gitter.HtmlToXaml.Helpers
{
    internal static class SpanHelper
    {
        public static void AddChildren(this Span s, HtmlNode node)
        {
            s.Inlines.AddChildren(node, NodeHelper.GenerateBlockForNode);
        }
    }
}

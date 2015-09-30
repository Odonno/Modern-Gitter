using System;
using Windows.UI.Xaml.Documents;
using Gitter.Common;
using HtmlAgilityPack;

namespace Gitter.Helpers
{
    public static class ParagraphHelper
    {
        public static Paragraph CreateEmptyParagraph()
        {
            var p = new Paragraph();
            HtmlToXaml.Blocks.Add(p);
            return p;
        }

        public static void GenerateParagraph(HtmlNode node)
        {
            var p = CreateEmptyParagraph();
            p.AddChildren(node);
        }

        public static void AddChildren(this Paragraph p, HtmlNode node, bool isCodeBlock = false)
        {
            if (isCodeBlock)
                p.Inlines.AddChildren(node, NodeHelper.GenerateCodeBlockForNode);
            else
                p.Inlines.AddChildren(node, NodeHelper.GenerateBlockForNode);
        }
    }
}

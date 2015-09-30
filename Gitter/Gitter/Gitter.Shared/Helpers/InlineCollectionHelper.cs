using System;
using System.Net;
using Windows.UI.Xaml.Documents;
using HtmlAgilityPack;

namespace Gitter.Helpers
{
    public static class InlineCollectionHelper
    {
        public static void AddChildren(this InlineCollection children, HtmlNode node, Func<HtmlNode, Inline> generateBlock)
        {
            bool added = false;

            foreach (var childNode in node.ChildNodes)
            {
                var i = generateBlock(childNode);
                if (i != null)
                {
                    children.Add(i);
                    added = true;
                }
            }

            if (!added)
            {
                children.Add(new Run
                {
                    Text = WebUtility.HtmlDecode(node.InnerText)
                });
            }
        }
    }
}

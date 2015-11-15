using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using HtmlAgilityPack;
using System.IO;

namespace Gitter.HtmlToXaml.Helpers
{
    internal static class NodeHelper
    {
        public static Inline GenerateBlockForNode(HtmlNode node)
        {
            switch (node.Name)
            {
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    return GenerateTitle(node, node.Name);
                case "strong":
                    return GenerateText(node, "bold");
                case "em":
                    return GenerateText(node, "italic");
                case "span":
                case "#text":
                case "div":
                    return GenerateText(node);
                case "br":
                    return GenerateLineReturn();
                case "p":
                case "P":
                    return GenerateInnerParagraph(node);
                case "img":
                case "IMG":
                    return GenerateImage(node);
                case "a":
                case "A":
                    if (node.ChildNodes.Count >= 1 && (node.FirstChild.Name == "img" || node.FirstChild.Name == "IMG"))
                        return GenerateImage(node.FirstChild);
                    return GenerateHyperLink(node);
                case "code":
                    return GenerateCode(node);
                case "pre":
                    GenerateFormattedCode(node);
                    return null;
                case "blockquote":
                    GenerateQuote(node);
                    return null;
                case "ul":
                    GenerateItemList(node);
                    return null;
                default:
                    Debug.WriteLine(node.Name);
                    return null;
            }
        }

        public static Inline GenerateCodeBlockForNode(HtmlNode node)
        {
            if (node.Name == "span" && node.ChildNodes.Count > 1)
            {
                var s = new Span();

                foreach (var childNode in node.ChildNodes)
                {
                    var children = GenerateCodeBlockForNode(childNode);
                    s.Inlines.Add(children);
                }

                return s;
            }

            string text = WebUtility.HtmlDecode(node.InnerText);
            var foregroundColor = RetrieveFormattedCodeColor(node, text);

            var content = new Run
            {
                Text = text,
                FontSize = 14,
                Foreground = new SolidColorBrush(foregroundColor)
            };

            return content;
        }

        private static Color RetrieveFormattedCodeColor(HtmlNode node, string text)
        {
            if (text == "function")
                return Color.FromArgb(255, 102, 217, 239);

            string @class = null;
            if (node.Attributes["class"] != null)
                @class = node.Attributes["class"].Value;

            switch (@class)
            {
                case "keyword":
                    return Color.FromArgb(255, 249, 38, 114);
                case "string":
                    return Color.FromArgb(255, 230, 219, 116);
                case "title":
                    return Color.FromArgb(255, 166, 226, 46);
                case "params":
                    return Colors.White;
                default:
                    return Colors.White;
            }
        }

        private static Inline GenerateTitle(HtmlNode node, string type)
        {
            var content = new Run { Text = WebUtility.HtmlDecode(node.InnerText) };
            int titleValue = Convert.ToInt32(type[1].ToString());

            switch (titleValue)
            {
                case 1:
                    content.FontSize = 44;
                    content.FontWeight = FontWeights.Bold;
                    break;
                case 2:
                    content.FontSize = 38;
                    content.FontWeight = FontWeights.Bold;
                    break;
                case 3:
                    content.FontSize = 32;
                    content.FontWeight = FontWeights.Bold;
                    break;
                case 4:
                    content.FontSize = 30;
                    content.FontStyle = FontStyle.Italic;
                    break;
                case 5:
                    content.FontSize = 28;
                    content.FontStyle = FontStyle.Italic;
                    break;
                case 6:
                    content.FontSize = 26;
                    content.FontStyle = FontStyle.Italic;
                    break;
            }

            return content;
        }

        private static Inline GenerateText(HtmlNode node, string @class = null, int fontSize = 18)
        {
            var content = new Run
            {
                Text = WebUtility.HtmlDecode(node.InnerText).Replace("\n", ""),
                FontSize = fontSize
            };

            if (!string.IsNullOrWhiteSpace(@class))
            {
                if (@class.Contains("bold"))
                    content.FontWeight = FontWeights.Bold;

                if (@class.Contains("italic"))
                    content.FontStyle = FontStyle.Italic;
            }

            if (node.Attributes["class"] != null)
            {
                @class = node.Attributes["class"].Value;

                if (@class.Contains("mention"))
                    content.FontStyle = FontStyle.Italic;

                if (@class.Contains("issue"))
                    return GenerateIssueLink(node);
            }

            return content;
        }

        private static Inline GenerateLineReturn()
        {
            return new LineBreak();
        }

        private static Inline GenerateImage(HtmlNode node)
        {
            string src = node.Attributes["src"].Value;

            var bitmapImage = new BitmapImage(new Uri(src));
            var image = new Image { Source = bitmapImage };

            image.Tapped += HtmlToXaml.ImageTapped;

            return new InlineUIContainer { Child = image };
        }

        private static Inline GenerateInnerParagraph(HtmlNode node)
        {
            var s = new Span();
            s.AddChildren(node);
            return s;
        }

        private static Inline GenerateHyperLink(HtmlNode node)
        {
            string link = node.Attributes["href"].Value;
            return GenerateHyperlink(node, link);
        }

        private static Inline GenerateIssueLink(HtmlNode node)
        {
            string issueNumber = node.Attributes["data-issue"].Value;
            string roomName = HtmlToXaml.RoomName;
            string link = Path.Combine("http://github.com/", roomName, "issues/", issueNumber);

            return GenerateHyperlink(node, link);
        }

        private static Inline GenerateHyperlink(HtmlNode node, string link)
        {
            var hyperlink = new Hyperlink { NavigateUri = new Uri(link) };
            hyperlink.Inlines.Add(new Run { Text = node.InnerText });

            return hyperlink;
        }

        private static Inline GenerateCode(HtmlNode node)
        {
            return new Run
            {
                Text = WebUtility.HtmlDecode(node.InnerText),
                Foreground = new SolidColorBrush(Colors.Black)
            };
        }

        private static void GenerateFormattedCode(HtmlNode node)
        {
            var p = HtmlToXaml.NextParagraph();
            p.Margin = new Thickness(12, 0, 0, 0);

            p.Inlines.Add(GenerateLineReturn());
            p.AddChildren(node.FirstChild, true);
            p.Inlines.Add(GenerateLineReturn());

            HtmlToXaml.NextParagraph();
        }

        private static void GenerateQuote(HtmlNode node)
        {
            var p = HtmlToXaml.NextParagraph();

            int blockquoteFontSize = 14;
            var content = GenerateText(node, "italic", blockquoteFontSize);

            p.Margin = new Thickness(12, 0, 0, 0);
            p.Inlines.Add(new Run
            {
                Text = "\"",
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyle.Italic,
                FontSize = blockquoteFontSize
            });
            p.Inlines.Add(content);
            p.Inlines.Add(new Run
            {
                Text = "\"",
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyle.Italic,
                FontSize = blockquoteFontSize
            });

            HtmlToXaml.NextParagraph();
        }

        private static void GenerateItemList(HtmlNode node)
        {
            var p = HtmlToXaml.NextParagraph();
            p.Margin = new Thickness(12, 0, 0, 0);

            var listElements = node.Descendants("li").ToArray();
            int elementCount = listElements.Length;

            for (int i = 0; i < elementCount; i++)
            {
                p.Inlines.Add(new Run { Text = "* " + listElements[i].InnerText });

                if (i < elementCount - 1)
                    p.Inlines.Add(GenerateLineReturn());
            }

            HtmlToXaml.NextParagraph();
        }
    }
}

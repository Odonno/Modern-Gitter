using System.Diagnostics;
using Windows.UI.Text;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Views;
using Gitter.ViewModel.Abstract;
using Microsoft.Practices.ServiceLocation;

namespace Gitter.Common
{
    public class Properties : DependencyObject
    {
        #region Dependency Properties

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(Properties),
            new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }
        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        #endregion


        #region Events

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = d as RichTextBlock;
            if (richTextBlock != null)
            {
                // Retrieve the HTML value for the message
                string html = e.NewValue as string;

                // Generate blocks
                GenerateBlocksForHtml(html);

                // Add the newly generated blocks to the RichTextBlock
                richTextBlock.Blocks.Clear();
                foreach (var block in _blocks)
                    richTextBlock.Blocks.Add(block);
            }
        }

        #endregion


        #region Fields

        private static List<Block> _blocks;

        #endregion


        #region Methods

        private static void GenerateBlocksForHtml(string html)
        {
            try
            {
                _blocks = new List<Block>();

                // Throw exception if HTML content does not exist
                if (string.IsNullOrEmpty(html))
                    throw new ArgumentNullException(nameof(html));

                // Create an HTML document to search for elements inside (div, span, etc...)
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Generate a complete paragraph based on the HTML content
                GenerateParagraph(htmlDoc.DocumentNode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                App.TelemetryClient.TrackException(ex);
            }
        }

        private static void GenerateParagraph(HtmlNode node)
        {
            var p = new Paragraph();
            AddChildren(p, node);
            _blocks.Add(p);
        }
        private static Paragraph CreateEmptyParagraph()
        {
            var p = new Paragraph();
            _blocks.Add(p);
            return p;
        }

        private static void AddChildren(Paragraph p, HtmlNode node)
        {
            bool added = false;
            foreach (var childNode in node.ChildNodes)
            {
                Inline i = GenerateBlockForNode(childNode);
                if (i != null)
                {
                    p.Inlines.Add(i);
                    added = true;
                }
            }
            if (!added)
            {
                p.Inlines.Add(new Run
                {
                    Text = WebUtility.HtmlDecode(node.InnerText)
                });
            }
        }
        private static void AddChildren(Span s, HtmlNode node)
        {
            bool added = false;
            foreach (var childNode in node.ChildNodes)
            {
                Inline i = GenerateBlockForNode(childNode);
                if (i != null)
                {
                    s.Inlines.Add(i);
                    added = true;
                }
            }
            if (!added)
            {
                s.Inlines.Add(new Run
                {
                    Text = WebUtility.HtmlDecode(node.InnerText)
                });
            }
        }
        private static void AddChildrenForCode(Paragraph p, HtmlNode node)
        {
            bool added = false;
            foreach (var childNode in node.ChildNodes)
            {
                Inline i = GenerateCodeBlockForNode(childNode);
                if (i != null)
                {
                    p.Inlines.Add(i);
                    added = true;
                }
            }
            if (!added)
            {
                p.Inlines.Add(new Run
                {
                    Text = WebUtility.HtmlDecode(node.InnerText)
                });
            }
        }

        private static Inline GenerateBlockForNode(HtmlNode node)
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
                default:
                    Debug.WriteLine(node.Name);
                    return null;
            }
        }

        private static Inline GenerateCodeBlockForNode(HtmlNode node)
        {
            if (node.Name == "span" && node.ChildNodes.Count > 1)
            {
                var s = new Span();

                foreach (var childNode in node.ChildNodes)
                {
                    var childContent = GenerateCodeBlockForNode(childNode);
                    s.Inlines.Add(childContent);
                }

                return s;
            }

            var content = new Run
            {
                Text = WebUtility.HtmlDecode(node.InnerText),
                FontSize = 14
            };

            Color foregroundColor;

            string @class = null;
            if (node.Attributes["class"] != null)
                @class = node.Attributes["class"].Value;

            switch (@class)
            {
                case "keyword":
                    foregroundColor = Color.FromArgb(255, 249, 38, 114);
                    break;
                case "string":
                    foregroundColor = Color.FromArgb(255, 230, 219, 116);
                    break;
                case "title":
                    foregroundColor = Color.FromArgb(255, 166, 226, 46);
                    break;
                case "params":
                    foregroundColor = Colors.White;
                    break;
                default:
                    foregroundColor = Colors.White;
                    break;
            }

            if (content.Text == "function")
                foregroundColor = Color.FromArgb(255, 102, 217, 239);

            content.Foreground = new SolidColorBrush(foregroundColor);
            return content;
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

            image.Tapped += (sender, args) =>
            {
                ServiceLocator.Current.GetInstance<IFullImageViewModel>().Source = src;
                ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo("FullImage");
            };

            return new InlineUIContainer { Child = image };
        }

        private static Inline GenerateInnerParagraph(HtmlNode node)
        {
            var s = new Span();
            AddChildren(s, node);
            return s;
        }

        private static Inline GenerateHyperLink(HtmlNode node)
        {
            var hyperlink = new Hyperlink
            {
                NavigateUri = new Uri(node.Attributes["href"].Value)
            };
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
            var p = CreateEmptyParagraph();
            p.Margin = new Thickness(12, 0, 0, 0);

            p.Inlines.Add(GenerateLineReturn());
            AddChildrenForCode(p, node.FirstChild);
            p.Inlines.Add(GenerateLineReturn());
        }

        private static void GenerateQuote(HtmlNode node)
        {
            int blockquoteFontSize = 14;
            var p = CreateEmptyParagraph();
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
        }

        #endregion
    }
}

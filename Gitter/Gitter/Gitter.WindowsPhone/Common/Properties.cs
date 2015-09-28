using System.Diagnostics;
using Windows.UI.Text;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Views;
using Gitter.ViewModel.Abstract;
using Microsoft.Practices.ServiceLocation;

namespace Gitter.Common
{
    public class Properties : DependencyObject
    {
        #region Properties

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached("Html",
            typeof(string), typeof(Properties),
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
            if (richTextBlock == null)
                return;

            // Generate blocks
            string xhtml = e.NewValue as string;
            var blocks = GenerateBlocksForHtml(xhtml);

            // Add the blocks to the RichTextBlock
            richTextBlock.Blocks.Clear();
            foreach (var block in blocks)
            {
                richTextBlock.Blocks.Add(block);
            }
        }

        #endregion


        #region Methods

        private static List<Block> GenerateBlocksForHtml(string xhtml)
        {
            var blocks = new List<Block>();

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(xhtml);

                var block = GenerateParagraph(htmlDoc.DocumentNode);
                blocks.Add(block);
            }
            catch (Exception ex)
            {
            }

            return blocks;
        }

        private static Block GenerateParagraph(HtmlNode node)
        {
            var p = new Paragraph();
            AddChildren(p, node);
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
                p.Inlines.Add(new Run { Text = WebUtility.HtmlDecode(node.InnerText) });
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
                s.Inlines.Add(new Run { Text = WebUtility.HtmlDecode(node.InnerText) });
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
                case "blockquote":
                    return GenerateQuote(node);
                default:
#if DEBUG
                    Debug.WriteLine(node.Name);
#endif
                    return null;
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

        private static Inline GenerateText(HtmlNode node, string @class = null)
        {
            var content = new Run { Text = WebUtility.HtmlDecode(node.InnerText) };

            if (!string.IsNullOrWhiteSpace(@class))
            {
                if (@class == "bold")
                    content.FontWeight = FontWeights.Bold;

                if (@class == "italic")
                    content.FontStyle = FontStyle.Italic;
            }

            if (node.Attributes["class"] != null)
            {
                @class = node.Attributes["class"].Value;

                if (@class == "mention")
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
            throw new NotImplementedException();
        }

        private static Inline GenerateQuote(HtmlNode node)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

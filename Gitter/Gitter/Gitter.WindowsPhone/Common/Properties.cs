﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

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
            foreach (HtmlNode child in node.ChildNodes)
            {
                Inline i = GenerateBlockForNode(child);
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
            foreach (HtmlNode child in node.ChildNodes)
            {
                Inline i = GenerateBlockForNode(child);
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
                case "#text":
                case "div":
                    return GenerateText(node);
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
                    else
                        return GenerateHyperLink(node);
            }

            return null;
        }

        private static Inline GenerateText(HtmlNode node)
        {
            return new Run { Text = WebUtility.HtmlDecode(node.InnerText) };
        }

        private static Inline GenerateImage(HtmlNode node)
        {
            var bitmapImage = new BitmapImage(new Uri(node.Attributes["src"].Value));

            var image = new Image();
            image.Source = bitmapImage;

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

        #endregion
    }
}
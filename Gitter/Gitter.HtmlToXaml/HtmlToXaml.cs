using Gitter.HtmlToXaml.Helpers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

namespace Gitter.HtmlToXaml
{
    public static class HtmlToXaml
    {
        #region Fields

        public static string RoomName { get; set; }
        public static List<Block> Blocks { get; set; }
        public static Paragraph CurrentParagraph { get; set; }
        public static TappedEventHandler ImageTapped { get; set; }

        #endregion


        #region Methods

        public static void Execute(string html)
        {
            try
            {
                // Reset properties
                Blocks = new List<Block>();
                CurrentParagraph = null;

                // Throw exception if HTML content does not exist
                if (string.IsNullOrEmpty(html))
                    throw new ArgumentNullException(nameof(html));

                // Create an HTML document to search for elements inside (div, span, etc...)
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Generate a complete paragraph based on the HTML content
                GenerateParagraphs(htmlDoc);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static Paragraph NextParagraph()
        {
            if (CurrentParagraph == null || CurrentParagraph.Inlines.Count > 0)
                CurrentParagraph = CreateNewParagraph();

            return CurrentParagraph;
        }

        private static void GenerateParagraphs(HtmlDocument htmlDoc)
        {
            NextParagraph();

            foreach (var childNode in htmlDoc.DocumentNode.ChildNodes)
            {
                var i = NodeHelper.GenerateBlockForNode(childNode);
                if (i != null)
                    CurrentParagraph.Inlines.Add(i);
            }
        }

        private static Paragraph CreateNewParagraph()
        {
            var p = new Paragraph();
            Blocks.Add(p);
            return p;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Documents;
using Gitter.Helpers;
using HtmlAgilityPack;

namespace Gitter.Common
{
    public static class HtmlToXaml
    {
        #region Fields

        public static List<Block> Blocks { get; set; }

        #endregion


        #region Public Methods

        public static void GenerateBlocksForHtml(string html)
        {
            try
            {
                Blocks = new List<Block>();

                // Throw exception if HTML content does not exist
                if (string.IsNullOrEmpty(html))
                    throw new ArgumentNullException(nameof(html));

                // Create an HTML document to search for elements inside (div, span, etc...)
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Generate a complete paragraph based on the HTML content
                ParagraphHelper.GenerateParagraph(htmlDoc.DocumentNode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                App.TelemetryClient.TrackException(ex);
            }
        }

        #endregion
    }
}

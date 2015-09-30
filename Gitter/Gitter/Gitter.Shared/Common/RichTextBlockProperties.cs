using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Gitter.Common
{
    public class RichTextBlockProperties : DependencyObject
    {
        #region Dependency Properties

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(RichTextBlockProperties),
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


        #region Property Changed Events

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = d as RichTextBlock;
            if (richTextBlock != null)
            {
                // Retrieve the HTML value for the message
                string html = e.NewValue as string;

                // Generate blocks from HTML
                HtmlToXaml.Execute(html);

                // Add the newly generated blocks to the RichTextBlock
                richTextBlock.Blocks.Clear();
                foreach (var block in HtmlToXaml.Blocks)
                    richTextBlock.Blocks.Add(block);
            }
        }

        #endregion
    }
}

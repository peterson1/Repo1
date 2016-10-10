using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Repo1.WPF45.SDK.PrintTools
{
    public class PrintPreviewer
    {
        const double PPI = 96;

        public static void FitToLongBond(string printJobDesc, params FrameworkElement[] frameworkElements)
        {
            var doc = new FixedDocument();
            var width = 8.5 * PPI;
            var height = 14 * PPI;
            MessageBox.Show("Layout optimized for printing on long bond paper.", 
                          "  Page Size: Long Bond paper", 
                             MessageBoxButton.OK, MessageBoxImage.Information);

            doc.DocumentPaginator.PageSize = new Size(width, height);

            foreach (var ctrl in frameworkElements)
            {
                var scale = Math.Min(width / ctrl.ActualWidth,
                                     height / ctrl.ActualHeight);

                ctrl.LayoutTransform = new ScaleTransform(scale, scale);

                var size = new Size(width, height);
                ctrl.Measure(size);
                ((UIElement)ctrl).Arrange(new Rect(new Point(0, 0), size));
                var pg = new FixedPage();
                pg.Width = doc.DocumentPaginator.PageSize.Width;
                pg.Height = doc.DocumentPaginator.PageSize.Height;

                var ctxt = ctrl.DataContext;
                var panl = ctrl.Parent as Panel;
                if (panl == null)
                    throw new InvalidCastException($"Parent control of {ctrl.Name} must be a type of ‹Panel›.");

                panl.Children.Remove(ctrl);
                pg.Children.Add(ctrl);
                ctrl.DataContext = ctxt;

                var pgContent = new PageContent();
                pgContent.Child = pg;
                doc.Pages.Add(pgContent);
            }

            var win = new PrintPreviewWindow1();
            win.vwr.Document = doc;
            win.Show();
        }
    }
}

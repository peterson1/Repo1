﻿using System;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Repo1.WPF45.SDK.PrintTools
{
    public static class ScaledElementPrinter
    {
        public static void PrintScaled(this FrameworkElement elm, string printJobDesc = "scaled_printout")
            => ScaledElementPrinter.AskToPrint(elm, printJobDesc);


        public static void Print(string printJobDesc, params FrameworkElement[] frameworkElements)
        {
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
            //new Action(() =>
            //{
                ExecutePrintMultiPageDoc(printJobDesc, frameworkElements);
            //}));
        }


        private static void ExecutePrintMultiPageDoc(string printJobDesc, params FrameworkElement[] frameworkElements)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() != true) return;

            var caps = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);
            var doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight);

            foreach (var ctrl in frameworkElements)
            {
                var scale = Math.Min(caps.PageImageableArea.ExtentWidth / ctrl.ActualWidth,
                                     caps.PageImageableArea.ExtentHeight / ctrl.ActualHeight);

                ctrl.LayoutTransform = new ScaleTransform(scale, scale);

                var size = new Size(caps.PageImageableArea.ExtentWidth, caps.PageImageableArea.ExtentHeight);
                ctrl.Measure(size);
                ((UIElement)ctrl).Arrange(new Rect(new Point(caps.PageImageableArea.OriginWidth, 
                                                             caps.PageImageableArea.OriginHeight), size));
                var pg    = new FixedPage();
                pg.Width  = doc.DocumentPaginator.PageSize.Width;
                pg.Height = doc.DocumentPaginator.PageSize.Height;

                var ctxt = ctrl.DataContext;
                var panl = ctrl.Parent as Panel;
                if (panl == null)
                    throw new InvalidCastException($"Parent control of {ctrl.Name} must be a type of ‹Panel›.");

                panl.Children.Remove(ctrl);
                pg.Children.Add(ctrl);
                ctrl.DataContext = ctxt;
                //ctrl.InvalidateVisual(); ctrl.UpdateLayout(); ctrl.Focus();
                //Dispatch(() => ctrl.UpdateLayout());

                var pgContent = new PageContent();
                ((IAddChild)pgContent).AddChild(pg);
                doc.Pages.Add(pgContent);

                //ctrl.Focus();
                //pg.Focus();
                pg.InvalidateVisual();
                Thread.Sleep(100);
            }

            dlg.PrintDocument(doc.DocumentPaginator, printJobDesc);
        }

        private static void Dispatch(Action action)
            => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                Thread.Sleep(10);
                action.Invoke();
            }));


        public static void AskToPrint(ContentPresenter content
                                    , double printScaleOffset
                                    , string printJobDesc = "Tab Content Visual")
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() != true) return;

            OverrideUserSettings(dlg);

            double scale;
            var pCaps = ScaleToFit1Page(content, dlg, out scale, printScaleOffset);

            dlg.PrintVisual(content, "First Fit to Page WPF Print");

            ResetVisualState(content, pCaps);
        }


        public static void AskToPrint(FrameworkElement ctrl
                                    , string printJobDesc = "Scaled Visual")
        {
            PrintDialog print = new PrintDialog();
            if (print.ShowDialog() != true) return;

            PrintCapabilities capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);

            double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / ctrl.ActualWidth,
                                    capabilities.PageImageableArea.ExtentHeight / ctrl.ActualHeight);

            ctrl.LayoutTransform = new ScaleTransform(scale, scale);

            Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            ctrl.Measure(sz);
            ((UIElement)ctrl).Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight),
                sz));

            ctrl.Focus();

            print.PrintVisual(ctrl, printJobDesc);
        }




        private static PrintCapabilities ScaleToFit1Page(ContentPresenter content,
            PrintDialog dlg, out double scale, double printScaleOffset)
        {
            var prCaps = dlg.PrintQueue.GetPrintCapabilities(dlg.PrintTicket);

            //get scale of the print wrt to screen of WPF visual
            scale = Math.Min(prCaps.PageImageableArea.ExtentWidth
                / content.ActualWidth, prCaps.PageImageableArea.ExtentHeight /
                    content.ActualHeight);

            scale += printScaleOffset;

            //Transform the Visual to scale
            content.LayoutTransform = new ScaleTransform(scale, scale);

            //get the size of the printer page
            Size sz = new Size(prCaps.PageImageableArea.ExtentWidth, prCaps.PageImageableArea.ExtentHeight);

            //update the layout of the visual to the printer page size.
            content.Measure(sz);
            content.Arrange(new Rect(new Point(prCaps.PageImageableArea.OriginWidth, prCaps.PageImageableArea.OriginHeight), sz));

            return prCaps;
        }


        private static void ResetVisualState(ContentPresenter objectToPrint, PrintCapabilities printCaps)
        {
            objectToPrint.Width = double.NaN;
            objectToPrint.UpdateLayout();
            objectToPrint.LayoutTransform = new ScaleTransform(1, 1);
            Size size = new Size(printCaps.PageImageableArea.ExtentWidth,
                                 printCaps.PageImageableArea.ExtentHeight);
            objectToPrint.Measure(size);
            objectToPrint.Arrange(new Rect(new Point(printCaps.PageImageableArea.OriginWidth,
                                  printCaps.PageImageableArea.OriginHeight), size));
        }


        private static void OverrideUserSettings(PrintDialog dlg)
        {
            dlg.PrintTicket.PageResolution = new PageResolution(PageQualitativeResolution.High);
        }
    }
}

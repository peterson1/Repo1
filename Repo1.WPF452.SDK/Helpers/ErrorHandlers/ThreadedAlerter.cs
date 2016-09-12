using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Repo1.Core.ns12.Helpers.ExceptionExtensions;
using Repo1.Core.ns12.Helpers.StringExtensions;

namespace Repo1.WPF452.SDK.Helpers.ErrorHandlers
{
    public class ThreadedAlerter
    {
        public static void CatchErrors(Application app, Action<string> errorLogger = null)
        {
            var msg = "";

            app.DispatcherUnhandledException += (s, e) => 
            {
                msg = VisualizeException("Dispatcher", e.Exception);
                errorLogger?.Invoke(msg);
                e.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) => 
            {
                msg = VisualizeException("CurrentDomain", e.ExceptionObject);
                errorLogger?.Invoke(msg);
            };

            TaskScheduler.UnobservedTaskException += (s, e) => 
            {
                msg = VisualizeException("TaskScheduler", e.Exception);
                errorLogger?.Invoke(msg);
            };
        }


        private static string VisualizeException(string thrower, object exceptionObj)
        {
            var shortMsg = ""; var longMsg = "";

            if (exceptionObj == null)
            {
                shortMsg = longMsg = $"NULL exception object received by global handler.";
                goto PreExit;
            }

            var ex = exceptionObj as Exception;
            if (ex == null)
            {
                shortMsg = longMsg = $"Non-exception object thrown: ‹{exceptionObj.GetType().Name}›";
                goto PreExit;
            }

            shortMsg = ex.Info(false, true);
            longMsg  = $"Error from ‹{thrower}›" + L.f + ex.Info(true, true);

            PreExit:
            ShowOnNewThread($"Error from ‹{thrower}›", shortMsg);
            return longMsg;
        }


        private static void ShowOnNewThread(string caption, string message)
        {
            new Thread(new ThreadStart(delegate
            {
                MessageBox.Show(message, caption, 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            )).Start();
        }
    }
}

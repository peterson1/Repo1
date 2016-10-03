using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.Extensions.StringExtensions;

namespace Repo1.WPF45.SDK.ErrorHandlers
{
    public class ThreadedAlerter
    {
        public static void CatchErrors(Application app, Action<string> errorLogger = null)
        {
            var msg = "";

            app.DispatcherUnhandledException += (s, e) =>
            {
                msg = Show(e.Exception, "Dispatcher");
                errorLogger?.Invoke(msg);
                e.Handled = true;
            };


            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                msg = Show(e.ExceptionObject, "CurrentDomain");
                errorLogger?.Invoke(msg);
            };


            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                msg = Show(e.Exception, "TaskScheduler");
                errorLogger?.Invoke(msg);
            };
        }


        public static string Show(object exceptionObj, string thrower)
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
            longMsg = $"Error from ‹{thrower}›" + L.f + ex.Info(true, true);

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

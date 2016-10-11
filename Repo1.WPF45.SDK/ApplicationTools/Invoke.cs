using System;
using System.Windows;
using System.Windows.Threading;

namespace Repo1.WPF45.SDK.ApplicationTools
{
    public class Invoke
    {
        public static void On (DispatcherPriority priority, Action action)
            => Application.Current.Dispatcher.BeginInvoke(priority, new Action(() =>
            {
                //Thread.Sleep(10);
                action.Invoke();
            }));
    }
}

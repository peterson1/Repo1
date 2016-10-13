using System.Windows;

namespace Repo1.WPF45.SDK.ErrorHandlers
{
    public class Alerter
    {
        public static dynamic Warn(string message, dynamic returnObj = null)
        {
            MessageBox.Show(message, "   Warning",
                MessageBoxButton.OK, MessageBoxImage.Warning);

            return returnObj;
        }
    }


    public class Alert
    {
        public static void Fail(string message)
        {
            MessageBox.Show(message, "   Operation Failed",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

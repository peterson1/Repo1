using System.Windows;

namespace Repo1.WPF452.SDK.Helpers
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
}

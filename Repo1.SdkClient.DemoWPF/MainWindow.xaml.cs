using System.Windows;
using Repo1.WPF45.SDK.Extensions.ApplicationExtensions;

namespace Repo1.SdkClient.DemoWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) 
            => App.Current.Relaunch();
    }
}

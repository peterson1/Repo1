using System.Windows;
using Repo1.WPF452.SDK.Helpers.WindowExtensions;

namespace Repo1.ExeUploader.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (a, b) =>
            {
                this.MakeDraggable();
            };
        }
    }
}

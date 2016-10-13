using System.Windows;

namespace Repo1.ExeUploader.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Application.Current.MainWindow = this;

            //Loaded += (a, b) =>
            //{
            //    this.MakeDraggable();
            //};
        }
    }
}

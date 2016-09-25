//using System.Windows;
//using System.Windows.Input;

//namespace Repo1.WPF452.SDK.Helpers.WindowExtensions
//{
//    public static class CommonWindowExtensions
//    {
//        public static void MakeDraggable(this Window win)
//        {
//            win.MouseDown -= Window_MouseDown_Handler;
//            win.MouseDown += Window_MouseDown_Handler;
//        }

//        private static void Window_MouseDown_Handler(object sender, MouseButtonEventArgs e)
//        {
//            if (e.ChangedButton != MouseButton.Left) return;
//            var win = sender as Window;
//            win?.DragMove();
//        }
//    }
//}

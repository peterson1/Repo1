using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Repo1.WPF452.SDK.Behaviours.WindowBehaviors
{
    public class DraggableBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    (s as Window)?.DragMove();
            };
        }
    }
}

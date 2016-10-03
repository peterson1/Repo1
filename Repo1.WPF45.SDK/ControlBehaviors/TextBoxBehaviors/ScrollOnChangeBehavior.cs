using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Repo1.WPF45.SDK.ControlBehaviors.TextBoxBehaviors
{
    public class ScrollOnChangeBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += (s, e) =>
            {
                AssociatedObject.ScrollToEnd();
            };
        }
    }
}

using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Repo1.WPF452.SDK.Behaviours.TextBoxBehaviors
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

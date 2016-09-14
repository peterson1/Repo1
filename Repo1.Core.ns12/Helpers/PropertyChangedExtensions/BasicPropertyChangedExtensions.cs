using System;
using System.ComponentModel;

namespace Repo1.Core.ns12.Helpers.PropertyChangedExtensions
{
    public static class BasicPropertyChangedExtensions
    {
        public static void Raise(this PropertyChangedEventHandler handlr, string propertyName, object sender = null)
            => handlr?.Invoke(sender, new PropertyChangedEventArgs(propertyName));

        public static void Raise(this EventHandler handlr, object sender = null)
            => handlr?.Invoke(sender, new EventArgs());
    }
}

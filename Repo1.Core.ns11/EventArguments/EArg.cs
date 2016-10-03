using System;

namespace Repo1.Core.ns11.EventArguments
{
    public class EArg<T> : EventArgs
    {
        public EArg(T data)
        {
            Data = data;
        }


        public T Data { get; }
    }


    public static class EArgExtensions
    {
        public static void Raise<T>(this EventHandler<EArg<T>> handler, T data, object sender = null)
            => handler?.Invoke(sender, new EArg<T>(data));
    }
}

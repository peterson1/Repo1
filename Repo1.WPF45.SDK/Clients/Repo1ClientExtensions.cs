using System;
using System.Windows;
using Repo1.Core.ns11.R1Clients;
using Repo1.WPF45.SDK.ErrorHandlers;

namespace Repo1.WPF45.SDK.Clients
{
    public static class Repo1ClientExtensions
    {
        public static void CatchErrors(this IRepo1Client repo1Client, Application app, Action<string> errorLogger = null)
            => ThreadedAlerter.CatchErrors(app, PostIssueThen(repo1Client, errorLogger));


        private static Action<string> PostIssueThen(IRepo1Client repo1Client, Action<string> errorLogger)
            => new Action<string>(async msg =>
            {
                await repo1Client.PostRuntimeError(msg);
                errorLogger.Invoke(msg);
            });
    }
}

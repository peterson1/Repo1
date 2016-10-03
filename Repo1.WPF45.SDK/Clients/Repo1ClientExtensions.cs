using System.Windows;
using Repo1.Core.ns11.R1Clients;
using Repo1.WPF45.SDK.ErrorHandlers;

namespace Repo1.WPF45.SDK.Clients
{
    public static class Repo1ClientExtensions
    {
        public static void CatchErrors(this IRepo1Client repo1Client, Application app)
            => ThreadedAlerter.CatchErrors(app);
    }
}

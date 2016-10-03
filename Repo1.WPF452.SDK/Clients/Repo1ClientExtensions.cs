using System.Windows;
using Repo1.Core.ns11.R1Clients;
using Repo1.WPF452.SDK.Helpers.ErrorHandlers;

namespace Repo1.WPF452.SDK.Clients
{
    public static class Repo1ClientExtensions
    {
        public static void CatchErrors(this IRepo1Client repo1Client, Application app)
            => ThreadedAlerter.CatchErrors(app);
    }
}

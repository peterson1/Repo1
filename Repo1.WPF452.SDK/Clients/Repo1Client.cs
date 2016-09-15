using System.Threading;
using System.Threading.Tasks;
using PropertyChanged;
using Repo1.Core.ns12.Clients;

namespace Repo1.WPF452.SDK.Clients
{
    [ImplementPropertyChanged]
    public class Repo1Client : Repo1ClientBase1
    {
        public Repo1Client(string userName, string password, string apiBaseURL) : base(userName, password, apiBaseURL)
        {
        }

        protected override void RunOnNewThread(Task task)
            => new Thread(async () => await task).Start();
    }
}

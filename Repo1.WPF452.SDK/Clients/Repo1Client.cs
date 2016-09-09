using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Repo1.Core.ns12.Clients;

namespace Repo1.WPF452.SDK.Clients
{
    public class Repo1Client : Repo1ClientBase1
    {
        public Repo1Client(string configFileName)
        {

        }

        protected override void RunOnNewThread(Task task)
            => new Thread(async () => await task).Start();
    }
}

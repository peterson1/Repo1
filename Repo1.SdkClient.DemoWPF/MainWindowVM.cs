using System;
using PropertyChanged;
using Repo1.Core.ns11.R1Clients;

namespace Repo1.SdkClient.DemoWPF
{
    [ImplementPropertyChanged]
    class MainWindowVM
    {
        public MainWindowVM(IRepo1Client repo)
        {
            repo.StatusChanged += (s, e) => AppendLog(e.Data);
            
            repo.UpdateInstalled += (s, e) 
                => UpdatesInstalled = true;

            repo.OnWarning = x => AppendLog(x);

            repo.StartUpdateCheckLoop();
        }

        public string    Status            { get; set; }
        public bool      UpdatesInstalled  { get; set; }


        private void AppendLog(string message) 
            => Status += Environment.NewLine + message;
    }
}

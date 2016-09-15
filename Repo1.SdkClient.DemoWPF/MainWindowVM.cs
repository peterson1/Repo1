using PropertyChanged;
using Repo1.Core.ns12.Clients;

namespace Repo1.SdkClient.DemoWPF
{
    [ImplementPropertyChanged]
    class MainWindowVM
    {
        public MainWindowVM(IRepo1Client repo)
        {
            repo.StatusChanged += (s, e) => Status = e.Data;
            repo.StartUpdateCheckLoop();
        }

        public string  Status  { get; private set; }
    }
}

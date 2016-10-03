using System.Windows.Input;
using Repo1.Core.ns11.R1Clients;
using Repo1.LegacyClient.DemoWPF.Configuration;
using Repo1.WPF452.SDK.Clients;
using Repo1.WPF452.SDK.InputCommands;

namespace Repo1.LegacyClient.DemoWPF
{
    class MainWindowVM
    {
        private IRepo1Client _repo;


        public MainWindowVM()
        {
            var cfg  = LegacyCfg.ReadAndParse();

            _repo = new Repo1Client(cfg.UniqueCfgKey);

            _repo.ReadLegacyCfg = () => LegacyCfg.Read();

            StartTrackingCmd = new RelayCommand(x 
                => _repo.StartSessionUpdateLoop(cfg.Username, cfg.Password));

            StartTrackingCmd.Execute(null);
        }

        public ICommand  StartTrackingCmd  { get; private set; }
    }
}

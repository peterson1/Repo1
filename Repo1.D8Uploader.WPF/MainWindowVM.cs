using System.Windows;
using PropertyChanged;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.RestClients;
using Repo1.D8Uploader.Lib45.ViewModels;

namespace Repo1.D8Uploader.WPF
{
    [ImplementPropertyChanged]
    class MainWindowVM : MainWindowVmBase
    {
        public MainWindowVM(UploaderCfg uploaderCfg, UploaderClient2 uploaderClient, DeleterClientBase deleterClient) : base(uploaderCfg, uploaderClient, deleterClient)
        {
        }

        protected override string AppNameSpace 
            => typeof(App).Namespace;

        protected override void SetClipboardText(string text)
            => Clipboard.SetText(text);
    }
}

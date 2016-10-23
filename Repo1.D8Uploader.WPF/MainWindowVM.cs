using Repo1.Core.ns11.InputCommands;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.D8Uploader.Lib45.Configuration;
using Repo1.D8Uploader.Lib45.RestClients;

namespace Repo1.D8Uploader.WPF
{
    class MainWindowVM
    {
        public MainWindowVM(UploaderCfg uploaderCfg, UploaderClient2 uploaderClient, DeleterClient2 deleterClient)
        {

        }

        public UploaderCfg      Config          { get; private set; }
        public UploaderClient2  Uploader        { get; private set; }
        public DeleterClient2   Deleter         { get; private set; }
        public string           Title           { get; private set; }
        public D8Package        LocalExe        { get; private set; }
        public D8Package        RemoteExe       { get; private set; }
        public IR1Command       RefreshCmd      { get; private set; }
        public IR1Command       UploadCmd       { get; private set; }
        public bool             HasChanges      { get; private set; }
        public string           VersionChanges  { get; set; }
        public double           MaxPartSizeMB   { get; set; } = 0.5;
    }
}

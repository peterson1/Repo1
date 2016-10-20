using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PropertyChanged;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.CollectionExtensions;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.InputCommands;
using Repo1.Core.ns11.R1Models;
using Repo1.Core.ns11.R1Models.ViewsLists;
using Repo1.ExeUploader.WPF.UI;
using Repo1.WPF45.SDK.Clients;
using Repo1.WPF45.SDK.ErrorHandlers;
using Repo1.WPF45.SDK.InputCommands;

namespace Repo1.ExeUploader.WPF.Clients
{
    [ImplementPropertyChanged]
    class DeleterClient1 : D7SvcStackClientBase
    {
        public DeleterClient1(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            RefreshCmd    = R1Command.Async(GetUploadedParts);
            ShowOldiesCmd = R1Command.Relay(ShowUploadedsWindow);
        }

        public R1Executable  Executable     { get; private set; }
        public IR1Command    RefreshCmd     { get; private set; }
        public IR1Command    ShowOldiesCmd  { get; private set; }

        public ObservableCollection<ExeVersionRowVM>  ExeVersions  { get; private set; } = new ObservableCollection<ExeVersionRowVM>();


        internal void Initialize (R1Executable r1Executable)
        {
            Executable = r1Executable;
            RefreshCmd.ExecuteIfItCan(null);
        }


        private void ShowUploadedsWindow()
        {
            var win         = new ExeVersionsWindow1();
            win.DataContext = this;
            win.Owner       = Application.Current.MainWindow;
            win.ShowDialog();
        }


        private async Task GetUploadedParts()
        {
            Status    = "Getting previously uploaded versions ...";
            var parts = await ViewsList<PartsUploadedByUser>(Executable.nid);
            var list  = new List<ExeVersionRowVM>();

            foreach (var grp in parts.GroupBy(x => x.ExeVersion))
            {
                var row = new ExeVersionRowVM(grp);
                row.DeleteCmd = R1Command.Async(
                           () => DeleteVersionParts(row), 
                            x => row.Parts.Count > 0);
                list.Add(row);
            }
            ExeVersions.Swap(list.OrderBy(x => x.PostDate));
            Status = $"{ExeVersions.Count} uploaded versions found.";
        }


        private async Task DeleteVersionParts(ExeVersionRowVM row)
        {
            var tmpList = row.Parts.ToList();
            for (int i = 0; i < tmpList.Count; i++)
            {
                var p = tmpList[i];
                row.Status = $"Deleting part {p.PartNumber} of {p.TotalParts} : “{p.FileName}” ...";
                if (!(await Delete(p.nid)))
                {
                    Alert.Fail($"Failed to delete {p.FileName}");
                    break;
                }
                row.Parts.Remove(p);
            }
            row.Status = $"All {tmpList.Count} parts deleted.";
        }


        protected override void OnError(Exception ex)
            => MessageBox.Show(ex.Info());
    }
}

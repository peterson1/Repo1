using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Repo1.Core.ns11.Configuration;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.InputCommands;
using Repo1.Core.ns11.R1Models.D8Models;
using Repo1.WPF45.SDK.Clients;
using Repo1.WPF45.SDK.InputCommands;

namespace Repo1.D8Uploader.Lib45.RestClients
{
    public abstract class DeleterClientBase : D8SvcStackClientBase
    {
        public DeleterClientBase(RestServerCredentials restServerCredentials) : base(restServerCredentials)
        {
            RefreshCmd    = R1Command.Async(GetUploadedParts);
            ShowOldiesCmd = R1Command.Relay(ShowUploadedsWindow);
            OnError       = ex => MessageBox.Show(ex.Info());
        }


        public D8Package    Package        { get; private set; }
        public IR1Command   RefreshCmd     { get; private set; }
        public IR1Command   ShowOldiesCmd  { get; private set; }

        //public ObservableCollection<ExeVersionRowVM> ExeVersions { get; private set; } = new ObservableCollection<ExeVersionRowVM>();


        protected abstract void ShowUploadedsWindow();


        public void Initialize(D8Package remotePkg)
        {
            Package = remotePkg;
            RefreshCmd.ExecuteIfItCan(null);
        }


        private async Task GetUploadedParts()
        {
            Status = "Getting previously uploaded versions ...";
            //var parts = await ViewsList<PartsUploadedByUser>(Executable.nid);
            //var list = new List<ExeVersionRowVM>();

            //foreach (var grp in parts.GroupBy(x => x.ExeVersion))
            //{
            //    var row = new ExeVersionRowVM(grp);
            //    row.DeleteCmd = R1Command.Async(
            //               () => DeleteVersionParts(row),
            //                x => row.Parts.Count > 0);
            //    list.Add(row);
            //}
            //ExeVersions.Swap(list.OrderBy(x => x.PostDate));
            //Status = $"{ExeVersions.Count} uploaded versions found.";
            await Task.Delay(1);
        }


        //private async Task DeleteVersionParts(ExeVersionRowVM row)
        //{
        //    var tmpList = row.Parts.ToList();
        //    for (int i = 0; i < tmpList.Count; i++)
        //    {
        //        var p = tmpList[i];
        //        row.Status = $"Deleting part {p.PartNumber} of {p.TotalParts} : “{p.FileName}” ...";
        //        if (!(await Delete(p.nid)))
        //        {
        //            Alert.Fail($"Failed to delete {p.FileName}");
        //            break;
        //        }
        //        row.Parts.Remove(p);
        //    }
        //    row.Status = $"All {tmpList.Count} parts deleted.";
        //}
    }
}

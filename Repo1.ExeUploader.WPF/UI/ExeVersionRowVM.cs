using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using Repo1.Core.ns11.Extensions.CollectionExtensions;
using Repo1.Core.ns11.R1Models;
using Repo1.Core.ns11.R1Models.ViewsLists;

namespace Repo1.ExeUploader.WPF.UI
{
    [ImplementPropertyChanged]
    class ExeVersionRowVM
    {
        public ExeVersionRowVM(IEnumerable<PartsUploadedByUser> parts)
        {
            var sampl  = parts.First();
            Version    = sampl.ExeVersion;
            PostDate   = sampl.PostDate;

            Parts.Swap(parts.Select  (x => x as R1SplitPart)
                            .OrderBy (x => x.PartNumber));
        }

        public string     Version     { get; }
        public DateTime   PostDate    { get; }
        public string     Status      { get; set; }
        public ICommand   DeleteCmd   { get; set; }

        public ObservableCollection<R1SplitPart> Parts { get; } = new ObservableCollection<R1SplitPart>();
    }
}

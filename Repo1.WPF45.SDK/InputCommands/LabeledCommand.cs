using System;
using PropertyChanged;

namespace Repo1.WPF45.SDK.InputCommands
{
    [ImplementPropertyChanged]
    public class LabeledCommand : RelayCommand
    {
        public LabeledCommand(string label, Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute)
        {
            Label = label;
        }


        public string Label { get; set; }
    }
}

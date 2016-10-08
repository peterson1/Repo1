﻿using System;
using System.Windows.Input;

namespace Repo1.Core.ns11.InputCommands
{
    public interface ICommandR1 : ICommand
    {
        string    CurrentLabel      { get; set; }
        bool      IsBusy            { get; }
        bool      IsCheckable       { get; set; }
        bool      IsChecked         { get; set; }
        bool      OverrideEnabled   { get; set; }
        bool      LastExecutedOK    { get; }
        DateTime  LastExecuteStart  { get; }
        DateTime  LastExecuteEnd    { get; }

        void ExecuteIfItCan(object param = null);
        //void RaiseCanExecuteChanged();
    }
}
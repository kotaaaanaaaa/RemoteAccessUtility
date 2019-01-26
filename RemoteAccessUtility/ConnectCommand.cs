using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace RemoteAccessUtility
{
    public class ConnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var rdpFilename = "RemoteAccessUtility.rdp";

            var option = new RdpOption();
            option.General.FullAddress = ((Environment)parameter).ConnectionAddress;
            option.Write(rdpFilename);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rdpFilename);
            Process.Start("mstsc", path);
        }
    }
}

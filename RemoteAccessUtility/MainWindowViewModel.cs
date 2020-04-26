using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace RemoteAccessUtility
{
    public class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<Environment> Environments
        {
            get => _environments;
            set => SetProperty(ref _environments, value);
        }
        public ObservableCollection<Environment> _environments = new ObservableCollection<Environment>();

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }
        public ObservableCollection<Account> _accounts = new ObservableCollection<Account>();


        public Setting SystemSetting
        {
            get => _systemSetting;
            set => SetProperty(ref _systemSetting, value);
        }
        public Setting _systemSetting = new Setting();

        public MainWindowViewModel() { }
    }

    [Obsolete(null, true)]
    public class MainWindowViewModelDesigner : MainWindowViewModel
    {
        public MainWindowViewModelDesigner()
        {

            var account = new Account();
            Accounts = new ObservableCollection<Account>
            {
                account,
            };

            Environments = new ObservableCollection<Environment>
            {
                new Environment
                {
                    HostName = "localhost",
                    ConnectionAddress = "127.0.0.1",
                    OsType = OperatingSystemType.Windows,
                    Account = account,
                    AccountGuid = account.Guid,
                },
            };
        }
    }
}
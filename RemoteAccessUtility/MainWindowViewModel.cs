using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RemoteAccessUtility
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            AddEnvironmentCommand = new DelegateCommand(async () => await AddEnvironment());
            EditEnvironmentCommand = new DelegateCommand<Environment>(async item => await EditEnvironment(item));
            RemoveEnvironmentCommand = new DelegateCommand<Environment>(async item => await RemoveEnvironment(item));

            SettingCommand = new DelegateCommand(async () => await Setting());
            EditAccountsCommand = new DelegateCommand(async () => await EditAccounts());
        }

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

        public void InitializeDb()
        {
            DbAccessor.InitializeSetting();
            DbAccessor.InitializeAccount();
            DbAccessor.InitializeEnvironment();

            SystemSetting = DbAccessor.SelectSetting();
            DbAccessor.SelectAccounts().ForEach(x => Accounts.Add(x));
            DbAccessor.SelectEnvironments().ForEach(env =>
            {
                var account = Accounts.Where(x => x.Guid == env.AccountGuid);
                if (account.Any())
                {
                    env.Account = account.First();
                }
                Environments.Add(env);
            });
        }

        private async Task AddEnvironment()
        {
            var item = new Environment();
            await EditEnvironment(item);
        }
        public DelegateCommand AddEnvironmentCommand { get; set; }

        private async Task EditEnvironment(Environment item)
        {
            var vm = new EnvironmentEditDialogViewModel(item, Accounts);
            var dialog = new EnvironmentEditDialog();
            dialog.DataContext = vm;
            var isSaved = (bool)await DialogHost.Show(dialog, "DialogHost");

            //refresh
            if (isSaved && Environments.Contains(item))
            {
                var index = Environments.IndexOf(item);
                Environments.Remove(item);
                Environments.Insert(index, item);
            }
            else if (isSaved)
            {
                Environments.Add(item);
            }
        }
        public DelegateCommand<Environment> EditEnvironmentCommand { get; set; }

        private async Task RemoveEnvironment(Environment item)
        {
            var vm = new EnvironmentEditDialogViewModel(item, Accounts);
            var dialog = new EnvironmentRemoveDialog();
            dialog.DataContext = vm;
            var result = (bool)await DialogHost.Show(dialog, "DialogHost");

            if (result)
            {
                DbAccessor.DeleteEnvironment(item);
                Environments.Remove(item);
            }
        }
        public DelegateCommand<Environment> RemoveEnvironmentCommand { get; set; }

        private async Task EditAccounts()
        {
            var vm = new AccountEditDialogViewModel(Accounts);
            vm.AccountsSelectedIndex = 0;
            var dialog = new AccountEditDialog();
            dialog.DataContext = vm;
            var result = await DialogHost.Show(dialog, "DialogHost");
        }
        public DelegateCommand EditAccountsCommand { get; set; }

        private async Task Setting()
        {
            var dialogView = new SettingDialog(SystemSetting);
            var result = await DialogHost.Show(dialogView, "DialogHost");
            var option = result as RdpOption;
            if (option != null)
            {
                SystemSetting.RdpOption = option;
                Environment.SystemRdpOption = SystemSetting.RdpOption;
            }
        }
        public DelegateCommand SettingCommand { get; set; }
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
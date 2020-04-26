using CoreUtilitiesPack;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteAccessUtility
{
    public class MainWindowViewModel : BindableBase
    {
        private static SqliteAccessor db = new SqliteAccessor();

        public MainWindowViewModel()
        {
            SettingCommand = new DelegateCommand(async () => await Setting());
            EditEnvironmentsCommand = new DelegateCommand(async () => await EditEnvironments());
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
            InitialzeSetting();
            InitialzeAccount();
            InitialzeEnvironment();

            SystemSetting = SelectSetting();
            SelectAccounts().ForEach(x => Accounts.Add(x));
            SelectEnvironments().ForEach(env =>
            {
                var account = Accounts.Where(x => x.Guid == env.AccountGuid);
                if (account.Any())
                {
                    env.Account = account.First();
                }
                Environments.Add(env);
            });
        }

        private void InitialzeSetting()
        {
            if (db.HasTable("setting"))
                return;

            var record = new Setting
            {
                RdpOption = new RdpOption(),
            };
            var sql = SqliteAccessor.GetCreateTableSQL("setting", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("setting", dic);
        }

        private void InitialzeAccount()
        {
            if (db.HasTable("account"))
                return;

            var record = new Account
            {
                Name = @"administrator",
                Password = @"password",
            };
            var sql = SqliteAccessor.GetCreateTableSQL("account", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("account", dic);
        }

        private void InitialzeEnvironment()
        {
            if (db.HasTable("environment"))
                return;

            var accounts = (IEnumerable<Account>)SelectAccounts();
            var record = new Environment
            {
                HostName = @"localhost",
                ConnectionAddress = @"127.0.0.1",
                OsType = OperatingSystemType.Windows,
                AccountGuid = accounts.First().Guid,
            };
            var sql = SqliteAccessor.GetCreateTableSQL("environment", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("environment", dic);
        }

        private async Task EditEnvironments()
        {
            var dialogView = new EnvironmentEditDialog(Environments, Accounts);
            var result = await DialogHost.Show(dialogView, "DialogHost");
            UpdatetEnvironments(Environments.ToList());
        }
        public DelegateCommand EditEnvironmentsCommand { get; set; }

        private async Task EditAccounts()
        {
            var dialogView = new AccountEditDialog(Accounts);
            var result = await DialogHost.Show(dialogView, "DialogHost");
            UpdatetAccounts(Accounts.ToList());
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

        public List<Account> SelectAccounts()
        {
            var sql = "select * from account";
            var records = new List<Account>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
        }

        public List<Environment> SelectEnvironments()
        {
            var sql = "select * from environment";
            var records = new List<Environment>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
        }

        public Setting SelectSetting()
        {
            var sql = "select * from setting";
            var records = new List<Setting>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records.First();
        }

        private void UpdatetAccounts(List<Account> accounts)
        {
            var target = SelectAccounts();
            foreach (var account in accounts)
            {
                if (target.Contains(account))
                {
                    target.Remove(account);
                }
            }
            db.ToDictionaries(target, out var deletes);
            db.Delete("account", deletes);

            db.ToDictionaries(accounts, out var upserts);
            db.Upserts("account", upserts);
        }

        private void UpdatetEnvironments(List<Environment> environments)
        {
            var target = SelectEnvironments();
            foreach (var environment in environments)
            {
                if (target.Contains(environment))
                {
                    target.Remove(environment);
                }
            }
            db.ToDictionaries(target, out var deletes);
            db.Delete("environment", deletes);

            db.ToDictionaries(environments, out var upserts);
            db.Upserts("environment", upserts);
        }
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
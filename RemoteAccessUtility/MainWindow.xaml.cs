using CoreUtilitiesPack;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RemoteAccessUtility.Icons;

namespace RemoteAccessUtility
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Environment> Environments = new List<Environment>();
        public List<Account> Accounts = new List<Account>();
        private static SqliteAccessor db = new SqliteAccessor();

        public MainWindow()
        {
            InitializeComponent();
            EnvironmentList.ItemsSource = Environments;

            InitializeDb();
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
        private void InitializeDb()
        {
            InitialzeAccount();
            InitialzeEnvironment();
        }

        private void InitialzeAccount()
        {
            if (db.HasTable("account"))
                return;

            var record = new Account()
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
            var record = new Environment()
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

        private List<Account> SelectAccounts()
        {
            var sql = "select * from account";
            var records = new List<Account>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
        }

        private List<Environment> SelectEnvironments()
        {
            var sql = "select * from environment";
            var records = new List<Environment>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
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


        private async void EnvironmentEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialogView = new EnvironmentEditDialog(Environments, Accounts);
            var result = await dialogHost.ShowDialog(dialogView);
            UpdatetEnvironments(Environments);
        }

        private async void AccountEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialogView = new AccountEditDialog(Accounts);
            var result = await dialogHost.ShowDialog(dialogView);
            UpdatetAccounts(Accounts);
        }

        private async void Setting_Click(object sender, RoutedEventArgs e)
        {
            var dialogView = new SettingDialog();
            var result = await dialogHost.ShowDialog(dialogView);
        }
    }
}

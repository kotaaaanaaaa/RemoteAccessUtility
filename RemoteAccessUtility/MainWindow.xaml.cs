﻿using CoreUtilitiesPack;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RemoteAccessUtility
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private static SqliteAccessor db = new SqliteAccessor();

        public MainWindow()
        {
            InitializeComponent();
            InitializeDb();

            var vm = new MainWindowViewModel();

            vm.SystemSetting = SelectSetting();
            Environment.SystemRdpOption = vm.SystemSetting.RdpOption;
            SelectAccounts().ForEach(x => vm.Accounts.Add(x));
            SelectEnvironments().ForEach(env =>
            {
                var account = vm.Accounts.Where(x => x.Guid == env.AccountGuid);
                if (account.Any())
                {
                    env.Account = account.First();
                }
                vm.Environments.Add(env);
            });
            DataContext = vm;
        }

        private void InitializeDb()
        {
            InitialzeSetting();
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

        private void InitialzeSetting()
        {
            if (db.HasTable("setting"))
                return;

            var record = new Setting()
            {
                RdpOption = new RdpOption(),
            };
            var sql = SqliteAccessor.GetCreateTableSQL("setting", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("setting", dic);
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

        private Setting SelectSetting()
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


        private async void EnvironmentEdit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainWindowViewModel) DataContext;
            var dialogView = new EnvironmentEditDialog(vm.Environments, vm.Accounts);
            var result = await dialogHost.ShowDialog(dialogView);
            UpdatetEnvironments(vm.Environments.ToList());
        }

        private async void AccountEdit_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            var dialogView = new AccountEditDialog(vm.Accounts);
            var result = await dialogHost.ShowDialog(dialogView);
            UpdatetAccounts(vm.Accounts.ToList());
        }

        private async void Setting_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            var dialogView = new SettingDialog(vm.SystemSetting);
            var result = await dialogHost.ShowDialog(dialogView);
            var option = result as RdpOption;
            if (option != null)
            {
                vm.SystemSetting.RdpOption = option;
                Environment.SystemRdpOption = vm.SystemSetting.RdpOption;
            }
        }
    }
}

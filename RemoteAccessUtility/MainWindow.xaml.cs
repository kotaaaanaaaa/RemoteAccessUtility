using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using CoreUtilitiesPack;
using MaterialDesignThemes.Wpf;

namespace RemoteAccessUtility
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Environment> Environments = new ObservableCollection<Environment>();
        public ObservableCollection<Account> Accounts = new ObservableCollection<Account>();
        private static SqliteAccessor db = new SqliteAccessor();

        public MainWindow()
        {
            InitializeComponent();
            EnvironmentList.ItemsSource = Environments;

            InitializeDb();
            SelectAccounts().ForEach(x => Accounts.Add(x));
            SelectEnvironments().ForEach(x => Environments.Add(x));
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
                PlainPassword = @"password",
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

            var record = new Environment()
            {
                HostName = @"localhost",
                ConnectionAddress = @"127.0.0.1",
                OsType = OperatingSystemType.Windows,
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

        private async void EnvironmentEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialogView = new EnvironmentEditDialog(Environments);
            var result = await dialogHost.ShowDialog(dialogView);
        }

        private async void AccountEdit_Click(object sender, RoutedEventArgs e)
        {
            var dialogView = new AccountEditDialog(Accounts);
            var result = await dialogHost.ShowDialog(dialogView);
        }

        private async void Setting_Click(object sender, RoutedEventArgs e)
        {
            var dialogView = new SettingDialog();
            var result = await dialogHost.ShowDialog(dialogView);
        }
    }
}

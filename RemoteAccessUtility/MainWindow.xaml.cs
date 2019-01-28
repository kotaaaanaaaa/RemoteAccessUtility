using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using CoreUtilitiesPack;

namespace RemoteAccessUtility
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Environment> Environments = new ObservableCollection<Environment>();
        private static SqliteAccessor db = new SqliteAccessor();

        public MainWindow()
        {
            InitializeComponent();
            EnvironmentList.ItemsSource = Environments;

            InitializeDb();

            var recoords = SelectEnvironments();
            foreach (var environment in recoords)
            {
                Environments.Add(environment);
            }
        }

        private void InitializeDb()
        {
            if (db.HasTable("environment"))
            {
                return;
            }

            var record = new Environment()
            {
                HostName = @"localhost",
                ConnectionAddress = @"127.0.0.1",
                OsType = Environment.OperatingSystemType.Windows,
            };
            var sql = SqliteAccessor.GetCreateTableSQL("environment", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("environment", dic);
        }

        private List<Environment> SelectEnvironments()
        {
            var sql = "select * from environment";
            var records = new List<Environment>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
        }
    }
}

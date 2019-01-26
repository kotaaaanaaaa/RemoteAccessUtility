using System.Collections.ObjectModel;
using System.Windows;

namespace RemoteAccessUtility
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Environment> Environments = new ObservableCollection<Environment>();

        public MainWindow()
        {
            InitializeComponent();
            EnvironmentList.ItemsSource = Environments;


            Environments.Add(new Environment()
            {
                HostName = @"localhost",
                ConnectionAddress = @"127.0.0.1",
                OsType = Environment.OperatingSystemType.Windows,
            });
        }
    }
}

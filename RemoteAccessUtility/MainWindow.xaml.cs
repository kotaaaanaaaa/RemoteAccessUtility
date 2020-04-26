using CoreUtilitiesPack;
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
            var vm = new MainWindowViewModel();
            vm.InitializeDb();
            Environment.SystemRdpOption = vm.SystemSetting.RdpOption;
            DataContext = vm;
        }
    }
}

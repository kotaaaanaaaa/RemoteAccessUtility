using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteAccessUtility
{
    /// <summary>
    /// SettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingDialog : UserControl
    {
        private Setting Setting;

        public SettingDialog(Setting setting)
        {
            InitializeComponent();
            Setting = setting;
            DataContext = Setting.RdpOption;
        }

        /// <summary>
        /// ダイアログの内容を保存する
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Setting.RdpOption.Parse(((RdpOption)DataContext).ToString());
        }
    }
}

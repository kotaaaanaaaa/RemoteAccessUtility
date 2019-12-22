using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace RemoteAccessUtility
{
    /// <summary>
    /// EnvironmentEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EnvironmentEditDialog : UserControl
    {
        private EnvironmentEditDialogViewModel vm = new EnvironmentEditDialogViewModel();

        public EnvironmentEditDialog(ObservableCollection<Environment> environments, IEnumerable<Account> accounts)
        {
            var envs = new ObservableCollection<Environment>();
            environments.ToList()
                .ForEach(x => envs.Add(new Environment(x)));

            vm.Source = environments;
            vm.Environments = envs;
            vm.Accounts = accounts;

            InitializeComponent();
            DataContext = vm;
            vm.EnvironmentsSelectedIndex = 0;
        }
    }
}

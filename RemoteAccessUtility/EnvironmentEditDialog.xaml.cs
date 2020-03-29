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

        public EnvironmentEditDialog(ObservableCollection<Environment> srcs, IEnumerable<Account> accounts)
        {
            var environments = new ObservableCollection<Environment>();
            srcs.ToList()
                .ForEach(x => environments.Add(new Environment(x)));

            vm.Source = srcs;
            vm.Environments = environments;
            vm.Accounts = accounts;

            InitializeComponent();
            DataContext = vm;
            vm.EnvironmentsSelectedIndex = 0;
        }
    }
}

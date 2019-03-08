using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RemoteAccessUtility
{
    /// <summary>
    /// EnvironmentEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EnvironmentEditDialog : UserControl
    {
        public ObservableCollection<EnvironmentEditDialogViewModel> EnvironmentViewModels;
        private List<Environment> Environments;
        private IEnumerable<Account> Accounts;

        public EnvironmentEditDialog(List<Environment> environments, IEnumerable<Account> accounts)
        {
            Environments = environments;
            EnvironmentViewModels = new ObservableCollection<EnvironmentEditDialogViewModel>();
            Environments.ForEach(x => { EnvironmentViewModels.Add(new EnvironmentEditDialogViewModel(x)); });
            Accounts = accounts;

            InitializeComponent();
            EnvironmentsList.ItemsSource = EnvironmentViewModels;
            EnvironmentsList.SelectedIndex = 0;
            AccountsList.ItemsSource = Accounts;
        }

        private async void EnvironmentsList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = (EnvironmentEditDialogViewModel)EnvironmentsList.SelectedItem;
            DataContext = selectedItem;
            if (selectedItem == null) return;

            var account = Accounts.Where(x => x.Guid == selectedItem.AccountGuid);
            if (account.Any())
                AccountsList.SelectedItem = account.First();
        }

        private async void AddEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var newEnvironment = new EnvironmentEditDialogViewModel();
            EnvironmentViewModels.Add(newEnvironment);
            EnvironmentsList.SelectedItem = newEnvironment;
        }

        private async void RemoveEnvironment_Click(object sender, RoutedEventArgs e)
        {
            if (EnvironmentViewModels.Count <= 1) return;
            var selectedItem = (EnvironmentEditDialogViewModel)EnvironmentsList.SelectedItem;
            if (selectedItem == null) return;
            EnvironmentsList.SelectedIndex = -1;
            EnvironmentViewModels.Remove(selectedItem);
            EnvironmentsList.SelectedIndex = 0;
        }

        private async void AccountsList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Account)AccountsList.SelectedItem;
            var viewModel = (EnvironmentEditDialogViewModel)EnvironmentsList.SelectedItem;
            viewModel.AccountGuid = selectedItem.Guid;
        }

        private async void HostName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var selectedItem = (EnvironmentEditDialogViewModel)DataContext;
            if (selectedItem == null) return;
            selectedItem.ValidateHostName(textBox.Text);
        }

        private async void ConnectionAddress_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var selectedItem = (EnvironmentEditDialogViewModel)DataContext;
            if (selectedItem == null) return;
            selectedItem.ValidateConnectionAddress(textBox.Text);
        }

        /// <summary>
        /// ダイアログの内容を保存する
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Environments.Clear();

            foreach (var vm in EnvironmentViewModels)
            {
                Environments.Add(new Environment()
                {
                    HostName = vm.HostName,
                    ConnectionAddress = vm.ConnectionAddress,
                    OsType = vm.OsType,
                    AccountGuid = vm.AccountGuid,
                });
            }
        }
    }
}

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
            EnvironmentsList.SelectedIndex = 0;
            EnvironmentViewModels.Remove(selectedItem);
        }
    }
}

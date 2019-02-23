using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RemoteAccessUtility
{
    /// <summary>
    /// AccountEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountEditDialog : UserControl
    {
        private IList<Account> Accounts;

        public AccountEditDialog(IList<Account> accounts)
        {
            Accounts = accounts;

            InitializeComponent();
            AccountsList.ItemsSource = Accounts;
            AccountsList.SelectedIndex = 0;
        }

        private async void AccountsList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Account)AccountsList.SelectedItem;
            if (selectedItem == null) return;
            this.DataContext = selectedItem;
            Password.Password = selectedItem.PlainPassword;
        }

        private async void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var newAccount = new Account();
            Accounts.Add(newAccount);
            AccountsList.SelectedItem = newAccount;
        }

        private async void RemoveAccount_Click(object sender, RoutedEventArgs e)
        {
            if (Accounts.Count <= 1) return;
            var selectedItem = (Account)AccountsList.SelectedItem;
            if (selectedItem == null) return;
            Accounts.Remove(selectedItem);
            AccountsList.SelectedIndex = 0;
        }
    }
}

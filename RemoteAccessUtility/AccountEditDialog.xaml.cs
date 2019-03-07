using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace RemoteAccessUtility
{
    /// <summary>
    /// AccountEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountEditDialog : UserControl
    {
        public ObservableCollection<AccountEditDialogViewModel> AccountViewModels;
        public List<Account> Accounts;
        private readonly char MaskChar = '●';
        private Guid PasswordEncryptGuid;
        private bool OnPasswordChanging = false;
        private Guid ConfirmEncryptGuid;
        private bool OnConfirmChanging = false;

        public AccountEditDialog(List<Account> accounts)
        {
            Accounts = accounts;
            AccountViewModels = new ObservableCollection<AccountEditDialogViewModel>();
            Accounts.ForEach(x => { AccountViewModels.Add(new AccountEditDialogViewModel(x, MaskChar)); });

            InitializeComponent();
            AccountsList.ItemsSource = AccountViewModels;
            AccountsList.SelectedIndex = 0;
        }

        private async void AccountsList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = (AccountEditDialogViewModel)AccountsList.SelectedItem;
            DataContext = selectedItem;

            Password.Select(selectedItem.Password.Length, 0);
            Confirm.Select(selectedItem.Confirm.Length, 0);
        }

        private async void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var newAccount = new AccountEditDialogViewModel();
            AccountViewModels.Add(newAccount);
            AccountsList.SelectedItem = newAccount;
        }

        private async void RemoveAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountViewModels.Count <= 1) return;
            var selectedItem = (AccountEditDialogViewModel)AccountsList.SelectedItem;
            if (selectedItem == null) return;
            AccountsList.SelectedIndex = 0;
            AccountViewModels.Remove(selectedItem);
        }

        private async void Password_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var selectedItem = (AccountEditDialogViewModel)DataContext;
            var now = selectedItem.Password;

            var newText = GetText(textBox, now);

            if (newText == now)
                return;
            selectedItem.Password = newText;

            if (OnPasswordChanging)
                return;

            if (now.IndexOf(newText) == 0)
                return;

            var newGuid = Guid.NewGuid();
            PasswordEncryptGuid = newGuid;
            Mask(textBox, newText, ref OnPasswordChanging, false);

            await Task.Delay(1000);
            if (!newGuid.Equals(PasswordEncryptGuid))
                return;
            Mask(textBox, newText, ref OnPasswordChanging, true);
        }

        private async void Confirm_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var selectedItem = (AccountEditDialogViewModel)DataContext;
            var now = selectedItem.Confirm;

            var newText = GetText(textBox, now);

            if (newText == now)
                return;
            selectedItem.Confirm = newText;

            if (OnConfirmChanging)
                return;

            if (now.IndexOf(newText) == 0)
                return;

            var newGuid = Guid.NewGuid();
            ConfirmEncryptGuid = newGuid;
            Mask(textBox, newText, ref OnConfirmChanging, false);

            await Task.Delay(1000);
            if (!newGuid.Equals(ConfirmEncryptGuid))
                return;
            Mask(textBox, newText, ref OnConfirmChanging, true);
        }

        /// <summary>
        /// マスクされたテキストボックスから文字列を取得する
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        private string GetText(TextBox textBox, string now)
        {
            var encryptedLength = textBox.Text.LastIndexOf(MaskChar) + 1;

            if (encryptedLength == 0)
                return textBox.Text;
            if (encryptedLength < textBox.Text.Length)
                return now.Substring(0, encryptedLength) + textBox.Text.Substring(encryptedLength);
            if (encryptedLength < now.Length)
                return now.Substring(0, encryptedLength);

            return now;
        }

        /// <summary>
        /// テキストボックスをマスクする
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="content"></param>
        /// <param name="all"></param>
        private void Mask(TextBox textBox, string content, ref bool changing, bool all = true)
        {
            string text;
            if (content.Length == 0)
                text = "";
            else if (all)
                text = "".PadLeft(content.Length, MaskChar);
            else
                text = "".PadLeft(content.Length - 1, MaskChar) + content.Substring(content.Length - 1);

            changing = true;
            textBox.Text = text;
            changing = false;
            textBox.Select(text.Length, 0);
        }

        /// <summary>
        /// ダイアログの内容を保存する
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Clear();

            foreach (var vm in AccountViewModels)
            {
                Accounts.Add(new Account()
                {
                    Name = vm.Name,
                    Password = vm.Password,
                    Guid = vm.Guid,
                });
            }
        }
    }
}

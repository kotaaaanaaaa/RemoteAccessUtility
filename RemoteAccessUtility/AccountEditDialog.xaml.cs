using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace RemoteAccessUtility
{
    /// <summary>
    /// AccountEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountEditDialog : UserControl
    {
        private IList<Account> Accounts;
        private readonly char MaskChar = '●';
        private Guid PasswordEncryptGuid;
        private bool OnPasswordChanging = false;
        private Guid ConfirmEncryptGuid;
        private bool OnConfirmChanging = false;

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

            var model = new AccountEditDialogViewModel();
            model.Name = selectedItem.Name;
            model.Password = selectedItem.Password;
            model.Confirm = selectedItem.Password;
            model.DisplayPassword = "".PadLeft(model.Password.Length, MaskChar);
            model.DisplayConfirm = "".PadLeft(model.Confirm.Length, MaskChar);
            DataContext = model;

            Password.Select(model.Password.Length, 0);
            Confirm.Select(model.Confirm.Length, 0);
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
    }
}

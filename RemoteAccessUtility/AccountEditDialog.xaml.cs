using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using ViewModel = RemoteAccessUtility.AccountEditDialogViewModel;

namespace RemoteAccessUtility
{
    /// <summary>
    /// AccountEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountEditDialog : UserControl
    {
        private ViewModel vm = new ViewModel();
        private Guid PasswordMaskingGuid;
        private Guid ConfirmMaskingGuid;

        public AccountEditDialog(ObservableCollection<Account> srcs)
        {
            var accounts = new ObservableCollection<Account>();
            srcs.ToList()
                .ForEach(x => accounts.Add(new Account(x)));

            vm.Source = srcs;
            vm.Accounts = accounts;

            InitializeComponent();
            DataContext = vm;
            vm.AccountsSelectedIndex = 0;
        }

        /// <summary>
        /// 名前変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Name_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            vm.Name = Name.Text;
        }

        /// <summary>
        /// パスワード変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Password_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var nowText = vm.Password;
            var newText = ViewModel.UnMask(Password.Text, nowText);

            //マスク時に再処理させない
            if (newText == nowText)
                return;

            var newGuid = Guid.NewGuid();
            PasswordMaskingGuid = newGuid;
            vm.Password = newText;
            vm.ValidatePassword();

            //削除時はマスクのみ
            if (nowText.IndexOf(newText) == 0)
            {
                Mask(Password, vm.Password, true);
                return;
            }

            //追加時は最後の1文字を一瞬表示
            Mask(Password, newText, false);
            await Task.Delay(1000);
            if (!newGuid.Equals(PasswordMaskingGuid))
                return;
            Mask(Password, vm.Password, true);
        }

        /// <summary>
        /// 確認用パスワード変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Confirm_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var nowText = vm.Confirm;
            var newText = ViewModel.UnMask(Confirm.Text, nowText);

            //マスク時に再処理させない
            if (newText == nowText)
                return;

            var newGuid = Guid.NewGuid();
            ConfirmMaskingGuid = newGuid;
            vm.Confirm = newText;
            vm.ValidatePassword();

            //削除時はマスクのみ
            if (nowText.IndexOf(newText) == 0)
            {
                Mask(Confirm, newText, true);
                return;
            }

            //追加時は最後の1文字を一瞬表示
            Mask(Confirm, newText, false);
            await Task.Delay(1000);
            if (!newGuid.Equals(ConfirmMaskingGuid))
                return;
            Mask(Confirm, newText, true);
        }

        /// <summary>
        /// テキストボックスをマスクする
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="content"></param>
        /// <param name="all"></param>
        private static void Mask(TextBox textBox, string content, bool all = true)
        {
            string text = ViewModel.Mask(content, all);
            textBox.Text = text;
            textBox.Select(text.Length, 0);
        }
    }
}

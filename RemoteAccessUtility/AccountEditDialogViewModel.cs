using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;
using System.Security;

namespace RemoteAccessUtility
{
    public class AccountEditDialogViewModel : BindableBase, INotifyDataErrorInfo
    {
        public ObservableCollection<Account> Source { get; set; }

        public static readonly char MaskChar = '●';


        public AccountEditDialogViewModel()
        {
            ValidateName();
            ValidatePassword();

            AddAccountCommand = new DelegateCommand(AddAccount, () => CanSave);
            RemoveAccountCommand = new DelegateCommand(RemoveAccount);
            AccountsChangedCommand = new DelegateCommand(AccountsChanged);
            SaveClickCommand = new DelegateCommand(SaveClick, () => CanSave);
        }

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }
        private ObservableCollection<Account> _accounts;

        public Account AccountsSelectedItem
        {
            get => _accountsSelectedItem;
            set
            {
                SetProperty(ref _accountsSelectedItem, value);
                _accountsSelectedIndex = Accounts.IndexOf(value);
                AccountsChanged();
            }
        }
        private Account _accountsSelectedItem;

        public int AccountsSelectedIndex
        {
            get => _accountsSelectedIndex;
            set
            {
                SetProperty(ref _accountsSelectedIndex, value);
                if (Accounts == null)
                    return;
                if (value <= -1 || Accounts.Count <= value)
                {
                    _accountsSelectedItem = null;
                }
                else
                {
                    _accountsSelectedItem = Accounts[value];
                }
                AccountsChanged();
            }
        }
        private int _accountsSelectedIndex;


        public DelegateCommand AccountsChangedCommand { get; set; }

        /// <summary>
        /// アカウントの選択変更時
        /// </summary>
        private void AccountsChanged()
        {
            if (AccountsSelectedItem == null)
            {
                Name = string.Empty;
                Password = string.Empty;
                Confirm = string.Empty;
                DisplayPassword = string.Empty;
                DisplayConfirm = string.Empty;
            }
            else
            {
                Name = AccountsSelectedItem.Name;
                Password = AccountsSelectedItem.Password;
                Confirm = AccountsSelectedItem.Password;
                DisplayPassword = Mask(Password);
                DisplayConfirm = Mask(Confirm);
            }
        }

        public DelegateCommand AddAccountCommand { get; set; }

        /// <summary>
        /// アカウントを追加する
        /// </summary>
        private void AddAccount()
        {
            var account = new Account();
            Accounts.Add(account);
            AccountsSelectedItem = account;
        }

        public DelegateCommand RemoveAccountCommand { get; set; }

        /// <summary>
        /// アカウントを削除する
        /// </summary>
        private void RemoveAccount()
        {
            var account = AccountsSelectedItem;
            AccountsSelectedIndex = -1;
            Accounts.Remove(account);
            AccountsSelectedIndex = 0;
        }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                AccountsSelectedItem.Name = value;

                ValidateName(_name);
            }
        }
        private string _name;

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get => new NetworkCredential("", _password).Password;
            set
            {
                var password = new NetworkCredential("", value).SecurePassword;
                if (password.Equals(_password))
                    return;
                _password = password;
                AccountsSelectedItem.Password = value;

                ValidatePassword();
            }
        }
        private SecureString _password;

        /// <summary>
        /// 確認用パスワード
        /// </summary>
        public string Confirm
        {
            get => new NetworkCredential("", _confirm).Password;
            set
            {
                var password = new NetworkCredential("", value).SecurePassword;
                if (password.Equals(_confirm))
                    return;
                _confirm = password;

                ValidatePassword();
            }
        }
        private SecureString _confirm;

        /// <summary>
        /// パスワード(表示用)
        /// </summary>
        public string DisplayPassword
        {
            get => _displayPassword;
            set
            {
                SetProperty(ref _displayPassword, value);
            }
        }
        private string _displayPassword = string.Empty;

        /// <summary>
        /// 確認用パスワード(表示用)
        /// </summary>
        public string DisplayConfirm
        {
            get => _displayConfirm;
            set
            {
                SetProperty(ref _displayConfirm, value);
            }
        }
        private string _displayConfirm = string.Empty;

        /// <summary>
        /// 保存可否を取得する
        /// </summary>
        public bool CanSave
        {
            get { return !HasErrors; }
        }

        /// <summary>
        /// テキストをマスクする
        /// </summary>
        /// <param name="plain"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static string Mask(string plain, bool all = true)
        {
            if (plain.Length == 0)
                return string.Empty;
            if (all)
                return string.Empty.PadLeft(plain.Length, MaskChar);
            return string.Empty.PadLeft(plain.Length - 1, MaskChar) + plain.Substring(plain.Length - 1);
        }

        /// <summary>
        /// テキストのマスクを解除する
        /// </summary>
        /// <param name="masked"></param>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static string UnMask(string masked, string plain)
        {
            var maskedLength = masked.LastIndexOf(MaskChar) + 1;

            if (maskedLength == 0)
                return masked;
            if (maskedLength < masked.Length)
                return plain.Substring(0, maskedLength) + masked.Substring(maskedLength);
            if (maskedLength < plain.Length)
                return plain.Substring(0, maskedLength);

            return plain;
        }

        #region Validataion

        /// <summary>
        /// ユーザー名を検証する
        /// </summary>
        public void ValidateName(string name = null)
        {
            var value = name ?? Name;
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(() => Name, "Fiels is required.");
            }
            else
            {
                RemoveError(() => Name);
            }
            RaisePropertyChanged("CanSave");
        }

        /// <summary>
        /// パスワードを検証する
        /// </summary>
        public void ValidatePassword()
        {
            if (Password == Confirm)
            {
                RemoveError(() => DisplayConfirm);
            }
            else
            {
                AddError(() => DisplayConfirm, "Confirm password must be same.");
            }
            RaisePropertyChanged("CanSave");
        }

        readonly Dictionary<string, List<string>> _currentErrors = new Dictionary<string, List<string>>();

        /// <summary>
        /// 検証エラーを追加する
        /// </summary>
        private void AddError<TResult>(Expression<Func<TResult>> propertyName, string error)
        {
            if (!(propertyName.Body is MemberExpression memberEx))
                throw new ArgumentException();

            if (!(memberEx.Expression is ConstantExpression senderExpression))
                throw new ArgumentException();

            var name = memberEx.Member.Name;

            if (!_currentErrors.ContainsKey(name))
                _currentErrors[name] = new List<string>();

            if (!_currentErrors[name].Contains(error))
            {
                _currentErrors[name].Add(error);
                OnErrorsChanged(name);
            }
        }

        /// <summary>
        /// 検証エラーを削除する
        /// </summary>
        private void RemoveError<TResult>(Expression<Func<TResult>> propertyName)
        {
            if (!(propertyName.Body is MemberExpression memberEx))
                throw new ArgumentException();

            if (!(memberEx.Expression is ConstantExpression senderExpression))
                throw new ArgumentException();

            var name = memberEx.Member.Name;

            if (_currentErrors.ContainsKey(name))
                _currentErrors.Remove(name);

            OnErrorsChanged(name);
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !_currentErrors.ContainsKey(propertyName))
                return null;

            return _currentErrors[propertyName];
        }

        public bool HasErrors
        {
            get => _currentErrors.Count > 0;
        }

        #endregion

        public DelegateCommand SaveClickCommand { get; set; }

        /// <summary>
        /// ダイアログの内容を保存する
        /// </summary>
        private void SaveClick()
        {
            Source.Clear();
            foreach (var account in Accounts)
            {
                Source.Add(new Account
                {
                    Name = account.Name,
                    Password = account.Password,
                    Guid = account.Guid,
                });
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net;
using System.Security;

namespace RemoteAccessUtility
{
    public class AccountEditDialogViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AccountEditDialogViewModel()
        {
            ValidateName();
            ValidatePassword();
            PropertyChanged.Raise(() => CanSave);
        }

        public AccountEditDialogViewModel(Account account, char maskChar) : this()
        {
            Name = account.Name;
            Password = account.Password;
            Confirm = account.Password;
            DisplayPassword = "".PadLeft(Password.Length, maskChar);
            DisplayConfirm = "".PadLeft(Confirm.Length, maskChar);
        }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                PropertyChanged.RaiseIfSet(() => Name, ref _name, value);

                ValidateName();
                PropertyChanged.Raise(() => CanSave);
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

                ValidatePassword();
                PropertyChanged.Raise(() => Password);

                PropertyChanged.Raise(() => CanSave);
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
                PropertyChanged.Raise(() => Confirm);

                PropertyChanged.Raise(() => CanSave);
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
                PropertyChanged.RaiseIfSet(() => DisplayPassword, ref _displayPassword, value);
            }
        }
        private string _displayPassword;

        /// <summary>
        /// 確認用パスワード(表示用)
        /// </summary>
        public string DisplayConfirm
        {
            get => _displayConfirm;
            set
            {
                PropertyChanged.RaiseIfSet(() => DisplayConfirm, ref _displayConfirm, value);
            }
        }
        private string _displayConfirm;

        public bool Equals(Account account)
        {
            if (Name != account.Name)
                return false;
            if (Password != account.Password)
                return false;

            return true;
        }

        /// <summary>
        /// 保存可否を取得する
        /// </summary>
        public bool CanSave
        {
            get { return !HasErrors; }
        }

        #region Validataion

        /// <summary>
        /// ユーザー名を検証する
        /// </summary>
        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(() => Name, "Fiels is required.");
            }
            else
            {
                RemoveError(() => Name);
            }
        }

        /// <summary>
        /// パスワードを検証する
        /// </summary>
        private void ValidatePassword()
        {
            if (Confirm == Password)
            {
                RemoveError(() => DisplayConfirm);
            }
            else
            {
                AddError(() => DisplayConfirm, "Confirm password must be same.");
            }
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
    }
}

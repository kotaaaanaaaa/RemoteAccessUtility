using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace RemoteAccessUtility
{
    public class EnvironmentEditDialogViewModel : BindableBase
    {
        private Environment nowEnv = new Environment();
        private Environment newEnv;

        public EnvironmentEditDialogViewModel(Environment item, IEnumerable<Account> accounts)
        {
            nowEnv = DbAccessor.Copy(item);
            newEnv = item;
            Accounts = accounts;

            ValidateHostName();
            ValidateConnectionAddress();

            HostNameChangedCommand = new DelegateCommand(() => ValidateHostName());
            ConnectionAddressChangedCommand = new DelegateCommand(() => ValidateConnectionAddress());
            AccountChangedCommand = new DelegateCommand(AccountChanged);
            SaveClickCommand = new DelegateCommand(SaveClick);

            var account = Accounts.FirstOrDefault(x => x.Guid == item.AccountGuid);
            if (account != null)
            {
                AccountsSelectedItem = account;
            }
            else
            {
                AccountsSelectedIndex = -1;
            }
        }

        /// <summary>
        /// ホスト名(表示用)
        /// </summary>
        public string HostName
        {
            get => newEnv.HostName;
            set
            {
                var hostName = newEnv.HostName;
                SetProperty(ref hostName, value);
                newEnv.HostName = hostName;
                ValidateHostName();
            }
        }

        public DelegateCommand HostNameChangedCommand { get; set; }

        /// <summary>
        /// RDP接続先アドレス
        /// </summary>
        public string ConnectionAddress
        {
            get => newEnv.ConnectionAddress;
            set
            {
                var address = newEnv.ConnectionAddress;
                SetProperty(ref address, value);
                newEnv.ConnectionAddress = address;
                ValidateConnectionAddress();
            }
        }

        public DelegateCommand ConnectionAddressChangedCommand { get; set; }

        /// <summary>
        /// OSの種類
        /// </summary>
        public OperatingSystemType OsType
        {
            get => newEnv.OsType;
            set
            {
                var osType = newEnv.OsType;
                SetProperty(ref osType, value);
                newEnv.OsType = osType;
            }
        }

        #region Accounts

        public IEnumerable<Account> Accounts { get; set; }

        public Account AccountsSelectedItem
        {
            get => Accounts.ElementAt(AccountsSelectedIndex);
            set
            {
                var index = -1;
                var list = Accounts.ToList();
                if (list.Contains(value))
                {
                    index = list.IndexOf(value);
                }
                AccountsSelectedIndex = index;
            }
        }

        public int AccountsSelectedIndex
        {
            get => _accountsSelectedIndex;
            set => SetProperty(ref _accountsSelectedIndex, value);
        }
        private int _accountsSelectedIndex;

        private void AccountChanged()
        {
            AccountGuid = AccountsSelectedItem.Guid;
        }
        public DelegateCommand AccountChangedCommand { get; set; }

        /// <summary>
        /// アカウントのGuid
        /// </summary>
        private string AccountGuid
        {
            get => newEnv.AccountGuid ?? string.Empty;
            set => newEnv.AccountGuid = value;
        }

        #endregion

        /// <summary>
        /// 保存可否を取得する
        /// </summary>
        public bool CanSave
        {
            get { return !HasErrors; }
        }

        #region Validataion

        /// <summary>
        /// ホスト名を検証する
        /// </summary>
        public void ValidateHostName(string hostName = null)
        {
            var value = hostName ?? HostName;
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(() => HostName, "Fiels is required.");
            }
            else
            {
                RemoveError(() => HostName);
            }
            RaisePropertyChanged("CanSave");
        }

        /// <summary>
        /// RDP接続先アドレスを検証する
        /// </summary>
        public void ValidateConnectionAddress(string connectionAddress = null)
        {
            var value = connectionAddress ?? ConnectionAddress;
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(() => ConnectionAddress, "Fiels is required.");
            }
            else
            {
                RemoveError(() => ConnectionAddress);
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

        /// <summary>
        /// ダイアログの内容を保存する
        /// </summary>
        private void SaveClick()
        {
            DbAccessor.DeleteEnvironment(nowEnv);
            DbAccessor.UpsertEnvironment(newEnv);
        }
        public DelegateCommand SaveClickCommand { get; set; }
    }
}

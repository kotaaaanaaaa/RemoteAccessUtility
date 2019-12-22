using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace RemoteAccessUtility
{
    public class EnvironmentEditDialogViewModel : BindableBase
    {
        public ObservableCollection<Environment> Source { get; set; }

        public EnvironmentEditDialogViewModel()
        {
            ValidateHostName();
            ValidateConnectionAddress();

            AddEnvironmentCommand = new DelegateCommand(AddEnvironment, () => CanSave);
            RemoveEnvironmentCommand = new DelegateCommand(RemoveEnvironment);
            EnvironmentChangedCommand = new DelegateCommand(EnvironmentChanged);
            AccountChangedCommand = new DelegateCommand(AccountChanged);
            HostNameChangedCommand = new DelegateCommand(HostNameChanged);
            ConnectionAddressChangedCommand = new DelegateCommand(ConnectionAddressChanged);
            SaveClickCommand = new DelegateCommand(SaveClick);
        }

        #region Environments

        public ObservableCollection<Environment> Environments
        {
            get => _environments;
            set => SetProperty(ref _environments, value);
        }
        public ObservableCollection<Environment> _environments;

        public Environment EnvironmentsSelectedItem
        {
            get => _environmentsSelected;
            set
            {
                SetProperty(ref _environmentsSelected, value);
                RaisePropertyChanged("HostName");
                RaisePropertyChanged("ConnectionAddress");
                RaisePropertyChanged("OsType");
                RaisePropertyChanged("AccountGuid");

                ValidateHostName();
                ValidateConnectionAddress();
            }
        }
        private Environment _environmentsSelected;

        public int EnvironmentsSelectedIndex
        {
            get => _environmentsSelectedIndex;
            set => SetProperty(ref _environmentsSelectedIndex, value);
        }

        private int _environmentsSelectedIndex;

        public DelegateCommand EnvironmentChangedCommand { get; set; }

        private void EnvironmentChanged()
        {
            var account = Accounts.Where(x => x.Guid == EnvironmentsSelectedItem.AccountGuid);
            if (account.Any())
            {
                AccountsSelectedItem = account.First();
            }
            else
            {
                AccountsSelectedIndex = -1;
            }
        }

        public DelegateCommand AddEnvironmentCommand { get; set; }

        private void AddEnvironment()
        {
            var env = new Environment();
            Environments.Add(env);
        }

        public DelegateCommand RemoveEnvironmentCommand { get; set; }

        private void RemoveEnvironment()
        {
            var env = EnvironmentsSelectedItem;
            EnvironmentsSelectedIndex = -1;
            Environments.Remove(env);
            EnvironmentsSelectedIndex = 0;
        }

        #endregion

        #region Accounts

        public IEnumerable<Account> Accounts { get; set; }

        public Account AccountsSelectedItem
        {
            get => _accountsSelected;
            set
            {
                SetProperty(ref _accountsSelected, value);
                RaisePropertyChanged("Accounts");
            }
        }
        private Account _accountsSelected = new Account();

        public int AccountsSelectedIndex
        {
            get => _accountsSelectedIndex;
            set => SetProperty(ref _accountsSelectedIndex, value);
        }
        private int _accountsSelectedIndex;

        public DelegateCommand AccountChangedCommand { get; set; }

        private void AccountChanged()
        {
            AccountGuid = _accountsSelected.Guid;
        }

        #endregion

        /// <summary>
        /// ホスト名(表示用)
        /// </summary>
        public string HostName
        {
            get => EnvironmentsSelectedItem != null ? EnvironmentsSelectedItem.HostName : string.Empty;
            set
            {
                var hostName = EnvironmentsSelectedItem.HostName;
                SetProperty(ref hostName, value);
                EnvironmentsSelectedItem.HostName = hostName;
                ValidateHostName();
            }
        }

        public DelegateCommand HostNameChangedCommand { get; set; }

        private void HostNameChanged()
        {
            ValidateHostName();
        }

        /// <summary>
        /// RDP接続先アドレス
        /// </summary>
        public string ConnectionAddress
        {
            get => EnvironmentsSelectedItem != null ? EnvironmentsSelectedItem.ConnectionAddress : string.Empty;
            set
            {
                var address = EnvironmentsSelectedItem.ConnectionAddress;
                SetProperty(ref address, value);
                EnvironmentsSelectedItem.ConnectionAddress = address;
                ValidateConnectionAddress();
            }
        }

        public DelegateCommand ConnectionAddressChangedCommand { get; set; }

        private void ConnectionAddressChanged()
        {
            ValidateConnectionAddress();
        }

        /// <summary>
        /// OSの種類
        /// </summary>
        public OperatingSystemType OsType
        {
            get => EnvironmentsSelectedItem?.OsType ?? OperatingSystemType.UnKnown;
            set
            {
                var osType = EnvironmentsSelectedItem.OsType;
                SetProperty(ref osType, value);
                EnvironmentsSelectedItem.OsType = osType;
            }
        }

        /// <summary>
        /// アカウントのGuid
        /// </summary>
        public string AccountGuid
        {
            get => EnvironmentsSelectedItem != null ? EnvironmentsSelectedItem.AccountGuid : string.Empty;
            set
            {
                var guid = EnvironmentsSelectedItem.AccountGuid;
                SetProperty(ref guid, value);
                EnvironmentsSelectedItem.AccountGuid = guid;
            }
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

        public DelegateCommand SaveClickCommand { get; set; }

        /// <summary>
        /// ダイアログの内容を保存する
        /// </summary>
        private void SaveClick()
        {
            Source.Clear();

            foreach (var env in Environments)
            {
                Source.Add(new Environment()
                {
                    HostName = env.HostName,
                    ConnectionAddress = env.ConnectionAddress,
                    OsType = env.OsType,
                    AccountGuid = env.AccountGuid,
                    Account = Accounts.First(x => env.AccountGuid.Equals(x.Guid)),
                });
            }
        }
    }
}

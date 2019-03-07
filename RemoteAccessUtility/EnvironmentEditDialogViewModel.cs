using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RemoteAccessUtility
{
    public class EnvironmentEditDialogViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public EnvironmentEditDialogViewModel()
        {
            ValidateHostName();
            ValidateConnectionAddress();
        }
        public EnvironmentEditDialogViewModel(Environment env) : this()
        {
            HostName = env.HostName;
            ConnectionAddress = env.ConnectionAddress;
            OsType = env.OsType;
            AccountGuid = env.AccountGuid;
        }

        /// <summary>
        /// ホスト名(表示用)
        /// </summary>
        public string HostName
        {
            get => _hostName;
            set
            {
                PropertyChanged.RaiseIfSet(() => HostName, ref _hostName, value);
                ValidateHostName();

            }
        }
        private string _hostName;

        /// <summary>
        /// RDP接続先アドレス
        /// </summary>
        public string ConnectionAddress
        {
            get => _connectionAddress;
            set
            {
                PropertyChanged.RaiseIfSet(() => ConnectionAddress, ref _connectionAddress, value);
                ValidateConnectionAddress();
            }
        }
        private string _connectionAddress;
        /// <summary>
        /// OSの種類
        /// </summary>
        public OperatingSystemType OsType
        {
            get => _osType;
            set
            {
                PropertyChanged.RaiseIfSet(() => OsType, ref _osType, value);
            }
        }
        private OperatingSystemType _osType;

        /// <summary>
        /// アカウントのGuid
        /// </summary>
        public string AccountGuid
        {
            get => _accountGuid;
            set
            {
                PropertyChanged.RaiseIfSet(() => AccountGuid, ref _accountGuid, value);
            }
        }
        private string _accountGuid;

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
            PropertyChanged.Raise(() => CanSave);
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
            PropertyChanged.Raise(() => CanSave);
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

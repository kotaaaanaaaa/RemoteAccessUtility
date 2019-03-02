using System.ComponentModel;

namespace RemoteAccessUtility
{
    public class EnvironmentEditDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public EnvironmentEditDialogViewModel(Environment env)
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
            }
        }
        private string _hostName;

        /// <summary>
        /// RDC接続先アドレス
        /// </summary>
        public string ConnectionAddress
        {
            get => _connectionAddress;
            set
            {
                PropertyChanged.RaiseIfSet(() => ConnectionAddress, ref _connectionAddress, value);
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
    }
}

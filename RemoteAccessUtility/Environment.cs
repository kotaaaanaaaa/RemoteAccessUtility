using System.ComponentModel;
using System.Windows.Input;

namespace RemoteAccessUtility
{
    public class Environment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand Connect { get; set; } = new ConnectCommand();

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

        public enum OperatingSystemType { UnKnown, Windows, Linux }

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
    }
}

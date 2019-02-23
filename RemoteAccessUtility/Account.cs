using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using CoreUtilitiesPack;

namespace RemoteAccessUtility
{
    public class Account : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Record(Name = "name", Type = RecordAttribute.FieldType.TEXT)]
        public string Name
        {
            get => _name;
            set
            {
                PropertyChanged.RaiseIfSet(() => Name, ref _name, value);
            }
        }
        private string _name;

        /// <summary>
        /// パスワード
        /// </summary>
        public SecureString Password
        {
            get => new NetworkCredential("", _password).SecurePassword;
            set
            {
                var password = new NetworkCredential("", value).Password;
                if (_password.Equals(password))
                    return;
                _password = password;
                PropertyChanged.Raise(() => Password);
            }
        }

        [Record(Name = "password", Type = RecordAttribute.FieldType.TEXT)]
        public string PlainPassword
        {
            get => _password;
            set
            {
                _password = value;
            }
        }
        private string _password;
    }
}

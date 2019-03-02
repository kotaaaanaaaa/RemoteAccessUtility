using CoreUtilitiesPack;
using System.Net;
using System.Security;

namespace RemoteAccessUtility
{
    public class Account
    {
        /// <summary>
        /// ユーザー名
        /// </summary>
        [Record(Name = "name", Type = RecordAttribute.FieldType.TEXT)]
        public string Name { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        [Record(Name = "password", Type = RecordAttribute.FieldType.TEXT)]
        public string Password
        {
            get => new NetworkCredential("", _password).Password;
            set
            {
                var password = new NetworkCredential("", value).SecurePassword;
                _password = password;
            }
        }
        private SecureString _password;

        /// <summary>
        /// GUID
        /// </summary>
        [Record(Name = "guid", Type = RecordAttribute.FieldType.TEXT)]
        public string Guid
        {
            get => _guid ?? System.Guid.NewGuid().ToString();
            set => _guid = value;
        }
        private string _guid = null;
    }
}

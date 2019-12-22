using CoreUtilitiesPack;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace RemoteAccessUtility
{
    public class Environment
    {
        public DelegateCommand ConnectCommand { get; set; }

        /// <summary>
        /// ホスト名(表示用)
        /// </summary>
        [Record(Name = "hostName", Type = RecordAttribute.FieldType.TEXT, Primary = true)]
        public string HostName { get; set; }

        /// <summary>
        /// RDC接続先アドレス
        /// </summary>
        [Record(Name = "connectionAddress", Type = RecordAttribute.FieldType.TEXT, Primary = true)]
        public string ConnectionAddress { get; set; }

        /// <summary>
        /// OSの種類
        /// </summary>
        public OperatingSystemType OsType { get; set; }
        [Record(Name = "osType", Type = RecordAttribute.FieldType.INTEGER)]
        private int OsTypeInt
        {
            get => (int)OsType;
            set => OsType = (OperatingSystemType)value;
        }

        /// <summary>
        /// アカウントのGuid
        /// </summary>
        [Record(Name = "accountGuid", Type = RecordAttribute.FieldType.TEXT)]
        public string AccountGuid { get; set; }

        /// <summary>
        /// アカウント
        /// </summary>
        public Account Account { get; set; }

        public static RdpOption SystemRdpOption { get; set; }

        public Environment()
        {
            ConnectCommand = new DelegateCommand(Connect);
        }

        public Environment(Environment env)
        {
            HostName = env.HostName;
            ConnectionAddress = env.ConnectionAddress;
            OsType = env.OsType;
            AccountGuid = env.AccountGuid;
        }

        /// <summary>
        /// 接続する
        /// </summary>
        private void Connect()
        {
            var rdpFilename = "RemoteAccessUtility.rdp";

            var option = SystemRdpOption;
            option.General.FullAddress = ConnectionAddress;
            option.General.Username = Account.Name;
            option.General.Password = Account.Password;
            option.Write(rdpFilename);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rdpFilename);
            Process.Start("mstsc", path);
        }
    }

    public enum OperatingSystemType
    {
        [Description("Unknown")]
        UnKnown,
        [Description("Windows")]
        Windows,
        [Description("Linux")]
        Linux
    }

    public static class EnumExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToString(this OperatingSystemType type)
        {
            var member = type.GetType().GetMember(type.ToString());
            var attributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = ((DescriptionAttribute)attributes[0]).Description;
            return description;
        }
    }
}

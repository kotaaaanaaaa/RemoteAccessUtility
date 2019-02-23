﻿using System.ComponentModel;
using System.Windows.Input;
using CoreUtilitiesPack;

namespace RemoteAccessUtility
{
    public class Environment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand Connect { get; set; } = new ConnectCommand();
        public ICommand Delete { get; set; } = new ConnectCommand();

        /// <summary>
        /// ホスト名(表示用)
        /// </summary>
        [Record(Name = "hostName", Type = RecordAttribute.FieldType.TEXT)]
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
        [Record(Name = "connectionAddress", Type = RecordAttribute.FieldType.TEXT)]
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

        [Record(Name = "osType", Type = RecordAttribute.FieldType.INTEGER)]
        private int OsTypeInt
        {
            get => (int)OsType;
            set => OsType = (OperatingSystemType)value;
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

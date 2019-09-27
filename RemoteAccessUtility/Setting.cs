using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreUtilitiesPack;

namespace RemoteAccessUtility
{
    public class Setting
    {

        [Record(Name = "rdpOption", Type = RecordAttribute.FieldType.TEXT)]
        private string _rdpOption { get; set; }

        public RdpOption RdpOption
        {
            get
            {
                var result = new RdpOption();
                result.Parse(_rdpOption);
                return result;
            }

            set { _rdpOption = value.ToString(); }
        }
    }
}

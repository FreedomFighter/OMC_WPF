using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_comm_lib
{
    /// <summary>
    /// 通信方式定义
    /// </summary>
    public enum CommunicateMode
    {
        RS232 = 0x01, SMS, GPRS, CSD, UDP, TCP, SNMP, NONE,
    }

    public class CommuEventArgs
    {
        private CommunicateMode _mode;
        public CommunicateMode Mode
        {
            get { return _mode; }
            private set { _mode = value; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }            
        }

        public CommuEventArgs(byte[] data, CommunicateMode mode)
        {
            Mode = mode;
            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }
    }

    public delegate void CommunDataReceiveHandler(object sender, CommuEventArgs e);
}

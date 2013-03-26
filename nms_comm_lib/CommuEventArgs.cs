using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace nms_comm_lib
{
    /// <summary>
    /// 通信方式定义
    /// </summary>
    public enum CommunicateMode
    {
        RS232 = 0x01, SMS, GPRS, CSD, UDP, TCP, SNMP, TIMEOUT, NONE,
    }

    public enum CommunicateLogs
    {
        LOG_TX, LOG_RX,
    }

    public class CommuEventArgs
    {
        private CommunicateMode _mode;
        public CommunicateMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private CommunicateLogs _logs;
        public CommunicateLogs Logs
        {
            get { return _logs; }
            set { _logs = value; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
        }

        private IPEndPoint _ipEndPoint = null;
        public IPEndPoint EndPoint
        {
            get { return _ipEndPoint; }
        }

        private TcpObject _gprsObject = null;
        public TcpObject GprsObject
        {
            get { return _gprsObject; }
        }

        private string _phoneText = string.Empty;
        public string PhoneText
        {
            get { return _phoneText; }
        }

        private string _commName = string.Empty;
        public string CommName
        {
            get { return _commName; }
        }

        private uint _siteID = 0x000000;
        public uint SiteID
        {
            get { return _siteID; }
        }

        private byte _subID = 0x00;
        public byte SubID
        {
            get { return _subID; }
        }
        /// <summary>
        /// 一般是本地串口发送接口
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public CommuEventArgs(byte[] data, CommunicateMode mode)
        {
            Mode = mode;
            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }

        public CommuEventArgs(byte[] data, string commName, CommunicateMode mode)
        {
            Mode = mode;
            _commName = commName;
            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }
        /// <summary>
        /// UDP发送接口
        /// </summary>
        /// <param name="data"></param>
        /// <param name="endPoint"></param>
        public CommuEventArgs(byte[] data, IPEndPoint endPoint)
        {
            Mode = CommunicateMode.UDP;
            _ipEndPoint = new IPEndPoint(endPoint.Address, endPoint.Port);

            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }
        /// <summary>
        /// 直接通过GPRS连接对象发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tcpObject"></param>
        public CommuEventArgs(byte[] data, TcpObject gprsObject)
        {
            Mode = CommunicateMode.GPRS;
            _gprsObject = gprsObject.Clone();

            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }
        /// <summary>
        /// 通过站点编号和设备编号查找GPRS连接对象发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        public CommuEventArgs(byte[] data, uint siteid, byte subid)
        {
            _siteID = siteid;
            _subID = subid;
            Mode = CommunicateMode.GPRS;

            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }
        /// <summary>
        /// modem通信方式，此方式下只需要一个电话号码和要发送的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="phone"></param>
        public CommuEventArgs(byte[] data, string phone)
        {
            _phoneText = phone;
            Mode = CommunicateMode.SMS;

            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }

        /// <summary>
        /// modem通信方式，此方式下只需要一个电话号码和要发送的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="phone"></param>
        public CommuEventArgs(byte[] data, string phone, string commName)
        {
            _phoneText = phone;
            _commName = commName;
            Mode = CommunicateMode.SMS;

            _data = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                _data[i] = data[i];
            }
        }
    }

    public delegate void CommunDataReceiveHandler(object sender, CommuEventArgs e);
}

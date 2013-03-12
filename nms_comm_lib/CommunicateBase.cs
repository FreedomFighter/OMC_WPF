using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace nms_comm_lib
{
    public class CommunicateBase
    {
        public event CommunDataReceiveHandler CommunDataReceiveComplated = null;

        List<UdpServer> udpServerList = new List<UdpServer>();
        List<TcpServer> tcpServerList = new List<TcpServer>();
        List<SmsMode> smsServerList = new List<SmsMode>();
        List<SerialMode> serialServerList = new List<SerialMode>();

        public CommunicateBase()
        {
            udpServerList.Clear();
            tcpServerList.Clear();
            smsServerList.Clear();
            serialServerList.Clear();
        }

        private void Stop()
        {
             DisponseUdpServer();
             DisponseTcpServer();
             DisponseSerialServer();
             DisponseModemServer();
        }

        /// <summary>
        /// 建立UDP服务器，用于UDP监控
        /// </summary>
        /// <param name="port"></param>
        /// <returns>成功返回true， 失败返回false</returns>
        public bool CreateUDPServer(ushort port)
        {
            bool bResult = false;

            UdpServer udpServer = new UdpServer(port);
            udpServer.UdpDataReceiveComplated += new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            bResult = udpServer.Start();
            if (bResult == true)
            {
                udpServerList.Add(udpServer);
            }

            return bResult;
        }
        /// <summary>
        /// 建立TCP服务器，用于GPRS监控
        /// </summary>
        /// <param name="port"></param>
        /// <returns>成功返回true， 失败返回false</returns>
        public bool CreateTCPServer(ushort port)
        {
            bool bResult = false;
            // 表示只能建立一个TCP服务器
            if (tcpServerList.Count > 0)
            {
                return bResult;
            }

            TcpServer tcpServer = new TcpServer(port);            
            tcpServer.TcpDataReceiveCompleted += new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            bResult = tcpServer.Start();
            if (bResult == true)
            {
                tcpServerList.Add(tcpServer);
            }

            return bResult;
        }

        public bool CreateRS232Server(string name, int baudrate)
        {
            bool bResult = false;

            SerialMode serialServer = new SerialMode(name, baudrate);
            serialServer.SerialDataReceiveComplated += new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            bResult = serialServer.Start();
            if (bResult == true)
            {
                serialServerList.Add(serialServer);
            }

            return bResult;
        }

        public bool CreateModemServer(string name, int baudrate)
        {
            bool bResult = false;

            SmsMode smsMode = new SmsMode(name, baudrate);
            smsMode.ModemDataRecevieCompleted += new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            bResult = smsMode.Start(3);
            if (bResult == true)
            {
                smsServerList.Add(smsMode);
            }

            return bResult;
        }

        public void DisponseUdpServer()
        {
            if (udpServerList.Count <= 0)
                return;

            foreach (UdpServer element in udpServerList)
            {
                element.Stop();
                element.UdpDataReceiveComplated -= new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            }
            udpServerList.Clear();
        }

        public void DisponseTcpServer()
        {
            if (tcpServerList.Count <= 0)
                return;

            foreach (TcpServer element in tcpServerList)
            {
                element.Stop();
                element.TcpDataReceiveCompleted -= new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            }
            tcpServerList.Clear();
        }

        public void DisponseSerialServer()
        {
            if (serialServerList.Count <= 0)
                return;

            foreach (SerialMode element in serialServerList)
            {
                element.Stop();
                element.SerialDataReceiveComplated -= new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            }
            serialServerList.Clear();
        }

        public void DisponseModemServer()
        {
            if (smsServerList.Count <= 0)
                return;

            foreach (SmsMode element in smsServerList)
            {
                element.Stop();
                element.ModemDataRecevieCompleted -= new CommunDataReceiveHandler(OnCommunDataReceiveComplated);
            }
            smsServerList.Clear();
        }

        private void OnCommunDataReceiveComplated(object sender, CommuEventArgs e)
        {            
            if (null != CommunDataReceiveComplated)
            {
                CommunDataReceiveComplated(sender, e);
            }
        }

        /// <summary>
        /// 所有接口的发送函数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="telphone"></param>
        /// <param name="iep"></param>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        /// <param name="mode"></param>
        private bool SendPrivate(byte[] data, string telphone, IPEndPoint iep, uint siteid, byte subid, CommunicateMode mode)
        {
            switch (mode)
            {
                case CommunicateMode.RS232:
                    if (serialServerList.Count <= 0)
                    {
                        return false;
                    }

                    SerialMode serialServer = (SerialMode)serialServerList[0];
                    if (null == serialServer)
                    {
                        return false;
                    }

                    serialServer.Send(data);
                    break;

                case CommunicateMode.CSD:
                    break;

                case CommunicateMode.GPRS:
                    if (tcpServerList.Count <= 0)
                    {
                        return false;
                    }

                    TcpServer tcpServer = (TcpServer)tcpServerList[0];
                    if (null == tcpServer)
                    {
                        return false;
                    }

                    tcpServer.Send(siteid, subid, data);
                    break;

                case CommunicateMode.SMS:
                    if (smsServerList.Count <= 0)
                    {
                        return false;
                    }
                    //规定第一个modem为专门发送，其他为接收
                    SmsMode smsMode = (SmsMode)smsServerList[0];
                    if (null == smsMode)
                    {
                        return false;
                    }

                    smsMode.SendToSMS(telphone, data, data.Length);
                    break;

                case CommunicateMode.SNMP:
                    break;
                case CommunicateMode.TCP:
                    break;

                case CommunicateMode.UDP:
                    if (udpServerList.Count <= 0)
                    {
                        return false;
                    }

                    UdpServer udpServer = (UdpServer)udpServerList[0];
                    if (null == udpServer)
                    {
                        return false;
                    }

                    udpServer.Send(data, iep);
                    break;
            }

            return true;
        }

        /// <summary>
        /// 所有接口的发送函数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="telphone"></param>
        /// <param name="iep"></param>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        /// <param name="mode"></param>
        private bool SendPrivate(string data, string telphone, IPEndPoint iep, uint siteid, byte subid, CommunicateMode mode)
        {
            switch (mode)
            {
                case CommunicateMode.RS232:
                    if (serialServerList.Count <= 0)
                    {
                        return false;
                    }

                    SerialMode serialServer = (SerialMode)serialServerList[0];
                    if (null == serialServer)
                    {
                        return false;
                    }

                    serialServer.Send(data);
                    break;

                case CommunicateMode.CSD:
                    break;

                case CommunicateMode.GPRS:
                    if (tcpServerList.Count <= 0)
                    {
                        return false;
                    }

                    TcpServer tcpServer = (TcpServer)tcpServerList[0];
                    if (null == tcpServer)
                    {
                        return false;
                    }

                    tcpServer.Send(siteid, subid, data);
                    break;

                case CommunicateMode.SMS:
                    if (smsServerList.Count <= 0)
                    {
                        return false;
                    }
                    //规定第一个modem为专门发送，其他为接收
                    SmsMode smsMode = (SmsMode)smsServerList[0];
                    if (null == smsMode)
                    {
                        return false;
                    }

                    smsMode.SendToSMS(telphone, data);
                    break;

                case CommunicateMode.SNMP:
                    break;
                case CommunicateMode.TCP:
                    break;

                case CommunicateMode.UDP:
                    if (udpServerList.Count <= 0)
                    {
                        return false;
                    }

                    UdpServer udpServer = (UdpServer)udpServerList[0];
                    if (null == udpServer)
                    {
                        return false;
                    }

                    udpServer.Send(data, iep);
                    break;
            }

            return true;
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为RS232
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(byte[] data)
        {
            return SendPrivate(data, null, null, 0, 0xFF, CommunicateMode.RS232);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为UDP
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(byte[] data, IPEndPoint iep)
        {
            return SendPrivate(data, null, iep, 0, 0xFF, CommunicateMode.UDP);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为MODEM
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(byte[] data, string telphone)
        {
            return SendPrivate(data, telphone, null, 0, 0xFF, CommunicateMode.SMS);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为MODEM
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(byte[] data, uint siteid, byte subid)
        {
            return SendPrivate(data, null, null, siteid, subid, CommunicateMode.GPRS);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为RS232
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(string data)
        {
            return SendPrivate(data, null, null, 0, 0xFF, CommunicateMode.RS232);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为UDP
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(string data, IPEndPoint iep)
        {
            return SendPrivate(data, null, iep, 0, 0xFF, CommunicateMode.UDP);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为MODEM
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(string data, string telphone)
        {
            return SendPrivate(data, telphone, null, 0, 0xFF, CommunicateMode.SMS);
        }

        /// <summary>
        /// 此函数只能发送串口的数据，通信方式必须设置为MODEM
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public bool Send(string data, uint siteid, byte subid)
        {
            return SendPrivate(data, null, null, siteid, subid, CommunicateMode.GPRS);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace nms_comm_lib
{
    class UdpServer
    {
        /// <summary>
        /// 本地监控端口号
        /// </summary>
        private ushort _localPort = 11000;
        public ushort LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }
        /// <summary>
        /// 标识该监控端口上是否已经开始
        /// </summary>
        private bool _isStart = false;
        public bool IsStart
        {
            get { return _isStart; }
            private set { _isStart = value; }
        }

        private Thread ServerThread { get; set; }
        private UdpClient ServerClient { get; set; }

        /// <summary>
        /// 数据接收完成事件，间数据上传至上层
        /// </summary>
        public event CommunDataReceiveHandler UdpDataReceiveComplated = null;
        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public UdpServer()
        {
        }
        /// <summary>
        /// 参数构造函数,本地监控端口号
        /// </summary>
        /// <param name="port"></param>
        public UdpServer(ushort port)
        {
            LocalPort = port;
        }
        /// <summary>
        /// 开始监控
        /// </summary>
        /// <returns>成功返回true， 失败返回false</returns>
        public bool Start()
        {
            return _Start(LocalPort);
        }
        /// <summary>
        /// 开始监控
        /// </summary>
        /// <param name="port"></param>
        /// <returns>成功返回true， 失败返回false</returns>
        public bool Start(ushort port)
        {
            return _Start(port);
        }
        /// <summary>
        /// 类函数内部开始函数
        /// </summary>
        /// <param name="port"></param>
        /// <returns>成功返回true， 失败返回false</returns>
        private bool _Start(ushort port)
        {
            if (_isStart == true)
            {
                return !_isStart;  // 返回false
            }

            try
            {
                _isStart = true;
                LocalPort = port;
                ServerClient = new UdpClient(LocalPort);
                ServerThread = new Thread(ServerThreadRoutine);
                ServerThread.Start();                
            }
            catch (Exception r)
            {
                _isStart = false;
                Console.WriteLine(r.Message);
            }

            return _isStart;
        }
        /// <summary>
        /// 停止UDP服务
        /// </summary>
        /// <returns>成功停止返回true，失败返回false</returns>
        public bool Stop()
        {
            if (false == _isStart)
            {
                return false; // 表示UDP服务没有开启
            }

            try
            {
                _isStart = false;
                ServerClient.Close();
                ServerThread.Abort();                
            }
            catch (Exception r)
            {
                _isStart = false;
                Console.WriteLine(r.Message);
            }

            return !_isStart; //成功停止返回true，失败返回false
        }

        /// <summary>
        /// 线程回调函数
        /// </summary>
        private void ServerThreadRoutine()
        {
            while (_isStart)
            {
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

                    if (ServerClient.Available > 1)
                    {
                        byte[] data = ServerClient.Receive(ref endPoint);

                        if (null != UdpDataReceiveComplated)
                        {
                            CommuEventArgs args = new CommuEventArgs(data, endPoint);
                            UdpDataReceiveComplated(this, args);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("UDP Server ServerThreadRoutine: " + e.Message);
                }
            }
        }

        /// <summary>
        /// 发送字符串数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="iep">无返回值</param>
        public void Send(string data, IPEndPoint iep)
        {
            if (false == _isStart || string.IsNullOrEmpty(data))
            {
                return;
            }

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                ServerClient.Send(bytes, bytes.Length, iep);
            }
            catch (Exception r)
            {
                Console.WriteLine("UDP Send: " + r.Message);
            }
        }
        /// <summary>
        /// 发送字串数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="iep">无返回值</param>
        public void Send(byte[] data, IPEndPoint iep)
        {
            if (false == _isStart || null == data)
            {
                return;
            }

            try
            {
                ServerClient.Send(data, data.Length, iep);
            }
            catch (Exception e)
            {
                Console.WriteLine("UDP Send: " + e.Message);
            }
        }
        /// <summary>
        /// 格式化IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns>返回 IPEndPoint类实例</returns>
        public IPEndPoint FormatAddressByString(string ip, ushort port)
        {
            IPAddress ipAddress = IPAddress.Any;

            if (false == IPAddress.TryParse(ip, out ipAddress))
            {
                return null;
            }

            IPEndPoint iep = new IPEndPoint(ipAddress, port);
            return iep;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nms_comm_lib
{
    /// <summary>
    /// Tcp客户端连接对象，每个连接自动生成对应的一个对象，对象通过站点ID和设备编号标识
    /// </summary>
    class TcpObject
    {
        public TcpClient Client { get; private set; }
        public BinaryReader Br { get; private set; }
        public BinaryWriter Bw { get; private set; }
        public NetworkStream Stream { get; private set; }

        public byte Subid { get; set; }
        public uint SiteId { get; set; }
        public Thread objThread { get; set; }
        public bool IsValid { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client"></param>
        public TcpObject(TcpClient client)
        {
            if (null == client)
            {
                return;
            }

            Client = client;

            Stream = Client.GetStream();
            if (null == Stream)
            {
                return;
            }

            Br = new BinaryReader(Stream);
            Bw = new BinaryWriter(Stream);

            this.IsValid = true;
        }
        /// <summary>
        /// 设置对象的站点编号和设备编号
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        public void SetSite(uint siteid, byte subid)
        {
            this.Subid = subid;
            this.SiteId = siteid;
        }

        public void Close()
        {
            Br.Close();
            Bw.Close();
            Client.Close();

            IsValid = false;
        }

        ~TcpObject()
        {
            Close();
        }
    }

    class TcpServer
    {
         /// <summary>
        /// 保存连接的所有设备 直放站 基站 网监仪 GPS设备等(符合协议的所有设备)
        /// 
        /// 属性默认为私有属性，不需要private关键字定义
        /// </summary>
        List<TcpObject> objList = new List<TcpObject>();
        public List<TcpObject> ClientList
        {
            get { return objList; }
        }
        /// <summary>
        /// 默认使用的本机IP地址
        /// </summary>
        IPAddress localAddress = IPAddress.Any;
        /// <summary>
        /// 默认监听端口号为11000，程序可以随意设置
        /// </summary>
        private int _localPort = 11000;
        public int LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }
        /// <summary>
        /// TCP监听器
        /// </summary>
        TcpListener tcpListener;
        /// <summary>
        /// 运行有效标志
        /// </summary>
        bool _isStart = false;
        public bool IsStart
        {
            get { return _isStart; }
        }
        /// <summary>
        /// TCP服务器监听线程
        /// </summary>
        Thread tcpThreadService;
                
        public event CommunDataReceiveHandler TcpDataReceiveCompleted = null;
       /// <summary>
       /// 无参构造函数
       /// </summary>
        public TcpServer()
        {

        }
        /// <summary>
        /// 构造函数,参数为本地监控端口号
        /// </summary>
        /// <param name="port"></param>
        public TcpServer(int port)
        {
            _localPort = port;
        }
        /// <summary>
        /// 回收资源，析构函数 
        /// </summary>
        ~TcpServer()
        {
            Stop();
        }
        /// <summary>
        /// 回收资源，停止TCP监听器工作，清除客户端列表
        /// </summary>
        public void Stop()
        {
            if (_isStart == false)
            {
                return;
            }

            try
            {
                _isStart = false;
                tcpListener.Stop();
                foreach (TcpObject element in objList)
                {
                    element.Close();
                }
                objList.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Stop: " + e.Message);
            }
        }
        /// <summary>
        /// 启动TCP监听线程，接收客户端连接请求
        /// </summary>
        public bool Start()
        {
            try
            {
                if (_isStart == true)
                {
                    return false;
                }

                _isStart = true;
                objList.Clear();
                tcpListener = new TcpListener(localAddress, _localPort);
                tcpListener.Start();
                tcpThreadService = new Thread(ClientConnectListener);
                tcpThreadService.Start();
            }
            catch (Exception e)
            {
                _isStart = false;
                Console.WriteLine("GPRS Start: " + e.Message);
            }

            return _isStart;
        }
        /// <summary>
        /// 启动TCP监听线程，接收客户端连接请求
        /// </summary>
        public bool Start(int port)
        {
            _localPort = port;

            return Start();
        }
        /// <summary>
        /// 监听客户端请求功能函数
        /// </summary>
        private void ClientConnectListener()
        {
            try
            {
                TcpClient newClient = null;
                while (true)
                {
                    newClient = tcpListener.AcceptTcpClient();
                    if (null != newClient)
                    {
                        //每接受一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息
                        TcpObject obj = new TcpObject(newClient);
                        obj.objThread = new Thread(TcpClientReceiveDataThread);
                        obj.objThread.Start(obj);
                        objList.Add(obj);                     
                    }

                    if (false == _isStart)
                    {
                        objList.Clear();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS ClientConnectListener: [string] " + e.Message);
            }
        }
        /// <summary>
        /// 每个连接客户端都通过此函数来接受数据
        /// </summary>
        private void TcpClientReceiveDataThread(object objState)
        {
            TcpObject obj = (TcpObject)objState;
            TcpClient client = obj.Client;

            try
            {
                while (obj.IsValid)
                {
                    int count = 0;
                    byte[] data = new byte[4096];

                    while ((count = obj.Stream.Read(data, 0, 4096)) != 0)
                    {
                        byte[] datas = new byte[count];
                        for (int iter = 0; iter < count; iter++)
                        {
                            datas[iter] = data[iter];
                        }

                        if (null != TcpDataReceiveCompleted)
                        {
                            CommuEventArgs args = new CommuEventArgs(datas, CommunicateMode.GPRS);
                            TcpDataReceiveCompleted(obj, args);
                        }
                    }

                    obj.Close();
                    objList.Remove(obj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS ReceiveData: " + e.Message);
            }
        }
        /// <summary>
        /// 发送字节流函数
        /// </summary>
        /// <param name="gprs"></param>
        /// <param name="data"></param>
        public void Send(TcpObject obj, byte[] data)
        {
            try
            {
                obj.Bw.Write(data);
                obj.Bw.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Send: [byte[]] " + e.Message);
            }
        }
        /// <summary>
        /// 发送字符串函数
        /// </summary>
        /// <param name="gprs"></param>
        /// <param name="data"></param>
        public void Send(TcpObject obj, string data)
        {
            try
            {
                obj.Bw.Write(data);
                obj.Bw.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Send: [string] " + e.Message);
            }
        }
        /// <summary>
        /// 根据站点ID和设备编号来发送数据
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        /// <param name="data"></param>
        public void Send(uint siteid, byte subid, string data)
        {
            try
            {
                foreach (TcpObject element in objList)
                {
                    if (siteid == element.SiteId && subid == element.Subid)
                    {
                        element.Bw.Write(data);
                        element.Bw.Flush();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Send: [string] " + e.Message);
            }
        }
        /// <summary>
        /// 根据站点ID和设备编号来发送数据
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        /// <param name="data"></param>
        public void Send(uint siteid, byte subid, byte[] data)
        {
            try
            {
                foreach (TcpObject element in objList)
                {
                    if (siteid == element.SiteId && subid == element.Subid)
                    {
                        element.Bw.Write(data);
                        element.Bw.Flush();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Send: [byte[]] " + e.Message);
            }
        }
        /// <summary>
        /// 广播字节流函数，将字节流广播到所有的客户端
        /// </summary>
        /// <param name="data"></param>
        public void Boardcast(byte[] data)
        {
            try
            {
                foreach (TcpObject element in objList)
                {
                    element.Bw.Write(data);
                    element.Bw.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Boardcast: [byte[]] " + e.Message);
            }
        }
        /// <summary>
        /// 广播字符串数据，所有客户端都能收到
        /// </summary>
        /// <param name="data"></param>
        public void Boardcast(string data)
        {
            try
            {
                foreach (TcpObject element in objList)
                {
                    element.Bw.Write(data);
                    element.Bw.Flush();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GPRS Boardcast: [string] " + e.Message);
            }
        }
    }
}

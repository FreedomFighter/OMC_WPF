using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;

namespace nms_comm_lib
{
    class SerialMode
    {
        /// <summary>
        /// 串口类定义
        /// </summary>
        private SerialPort serialPort = new SerialPort();
        public SerialPort SerialPort
        {
            get { return serialPort; }
        }
        /// <summary>
        /// 串口波特率
        /// </summary>
        public int BaudRate
        {
            get { return serialPort.BaudRate; }
            set { serialPort.BaudRate = value; }
        }
        /// <summary>
        /// 串口名称
        /// </summary>
        public string Name
        {
            get { return serialPort.PortName; }
            set { serialPort.PortName = value; }
        }

        private int timeout = 30;
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        private int SendTimeout = 0;
        private bool isStartSend = false;
       
        /// <summary>
        /// 线程ID
        /// </summary>
        private Thread SerialThread;
        /// <summary>
        /// 标识是否开始
        /// </summary>
        private bool _isStart = false;
        public bool IsStart
        {
            get { return _isStart; }
        }

        /* 串口接收定时器 */
        System.Timers.Timer serialTimer = null;
        // 接收串口数据的缓冲区
        private List<byte> commReceiving = new List<byte>();
        // 待发数据和已接收数据的列表
        private List<byte[]> commToSend = new List<byte[]>();
        private List<byte[]> commReceived = new List<byte[]>();
        private List<byte[]> commTimeout = new List<byte[]>();

        public event CommunDataReceiveHandler SerialDataReceiveComplated = null;
        public event CommunDataReceiveHandler SerialLogsReceiveComplated = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialMode()
        {
            _Start();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baudrate"></param>
        public SerialMode(string name, int baudrate, int timeout)
        {
            Name = name;
            Timeout = timeout;
            BaudRate = baudrate;

            _Start();
        }

        ~SerialMode()
        {
            Stop();
        }

        private void _Start()
        {
            SendTimeout = 0;
            isStartSend = false;

            // 初始化串口对象
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            serialTimer = new System.Timers.Timer();
            serialTimer.Interval = 100; // 100ms内没有收到数据则代表整包数据接收完成
            serialTimer.Elapsed += new ElapsedEventHandler(OnElapsedTimerHandler);
        }

        public bool Start()
        {
            try
            {
                _isStart = true;
                SerialThread = new Thread(OnRecevieThreadHandler);
                SerialThread.Start();
                serialPort.Open();
         
                // 接收串口数据的缓冲区
                commReceiving.Clear();
                // 已接收数据的列表
                commReceived.Clear();
            }
            catch (Exception e)
            {
                _isStart = false;
                Console.WriteLine("SerialServer Start" + e.Message);
            }

            return _isStart;
        }

        public bool Stop()
        {
            try
            {
                if (_isStart == true)
                {
                    _isStart = false;
                    SerialThread.Abort();
                    serialPort.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Serial Stop" + e.Message);
            }

            return _isStart;
        }

        private void OnElapsedTimerHandler(object sender, ElapsedEventArgs e)
        {
            //停止定时器
            serialTimer.Enabled = false;

            lock (commReceiving)
            {
                if (commReceiving.Count > 0)
                {
                    commReceived.Add(commReceiving.ToArray());
                    commReceiving.Clear();
                }
            }
        }

        private void OnRecevieThreadHandler()
        {
            while (_isStart)
            {
                try
                {
                    // 计算发送数据超时
                    lock (commTimeout)
                    {
                        if (isStartSend == true && commTimeout.Count > 0)
                        {
                            // 超时
                            if (++SendTimeout > Timeout * 10)
                            {
                                if (null != SerialDataReceiveComplated)
                                {
                                    byte[] data = commTimeout[0];
                                    CommuEventArgs args = new CommuEventArgs(data, CommunicateMode.TIMEOUT);
                                    SerialDataReceiveComplated(this, args);
                                    commTimeout.Clear();
                                    // 超时时恢复
                                    SendTimeout = 0;
                                    isStartSend = false;
                                }
                            }
                        }
                    }

                    lock (commReceived)
                    {
                        if (commReceived.Count > 0)
                        {
                            if (null != SerialDataReceiveComplated)
                            {
                                byte[] data = commReceived[0];
                                CommuEventArgs args = new CommuEventArgs(data, CommunicateMode.RS232);
                                SerialDataReceiveComplated(this, args);
                                commReceived.RemoveAt(0);
                                // 接收到数据时恢复
                                SendTimeout = 0;
                                isStartSend = false;

                                if (null != SerialLogsReceiveComplated)
                                {
                                    CommuEventArgs logs = new CommuEventArgs(data, this.Name, CommunicateMode.RS232);
                                    logs.Logs = CommunicateLogs.LOG_RX;
                                    SerialLogsReceiveComplated(this, logs);
                                }
                            }
                        }
                    }

                    lock (commToSend)
                    {
                        if (commToSend.Count > 0)
                        {
                            byte[] data = commToSend[0];
                            if (null == data || data.Length <= 0)
                            {
                                commToSend.RemoveAt(0);
                                continue;
                            }
                            if (null == serialPort)
                            {
                                continue;
                            }

                            serialPort.Write(data, 0, data.Length);
                            SendTimeout = 0;
                            isStartSend = true; // 标识以及发送数据，开始计数计算超时时间

                            if (null != SerialLogsReceiveComplated)
                            {
                                CommuEventArgs logs = new CommuEventArgs(data, this.Name, CommunicateMode.RS232);
                                logs.Logs = CommunicateLogs.LOG_TX;
                                SerialLogsReceiveComplated(this, logs);
                            }

                            commTimeout.Add(data);
                            commToSend.RemoveAt(0);                           
                        }
                    }

                    Thread.Sleep(100); //100ms
                }
                catch (Exception r)
                {
                    Console.WriteLine(r.Message);
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (serialPort)
            {
                if (serialPort.BytesToRead > 0) // 避免阻塞
                {
                    byte[] buffer = new byte[serialPort.BytesToRead];
                    serialPort.Read(buffer, 0, buffer.Length);

                    foreach (byte element in buffer)
                    {
                        commReceiving.Add(element);
                    }

                    //  开始定时器
                    serialTimer.Enabled = false;
                    serialTimer.Enabled = true;
                }
            }
        }
              
        public void Send(byte[] data)
        {
            lock (commToSend)
            {
                commToSend.Add(data);
            }
        }

        public void Send(string data)
        {
            lock (commToSend)
            {
                commToSend.Add(ASCIIEncoding.ASCII.GetBytes(data));
            }
        }

        public void Send(byte data)
        {
            lock (commToSend)
            {
                commToSend.Add(new byte[] { data });
            }
        }
    }
}

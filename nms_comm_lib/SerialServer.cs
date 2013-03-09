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
    class SerialServer
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

        public event CommunDataReceiveHandler SerialDataReceiveComplated = null;

        public SerialServer()
        {
            _Start();
        }

        public SerialServer(string name, int baudrate)
        {
            Name = name;
            BaudRate = baudrate;

            _Start();
        }

        ~SerialServer()
        {
            Stop();
        }

        private void _Start()
        {
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
                        }
                    }
                }

                lock (commToSend)
                {
                    if (commToSend.Count > 0)
                    {
                        byte[] data = commToSend[0];
                        serialPort.Write(data, 0, data.Length);
                        commToSend.RemoveAt(0);
                    }
                }

                Thread.Sleep(200); //100ms
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

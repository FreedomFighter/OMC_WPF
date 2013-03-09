using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nms_comm_lib
{
    class ModemServer
    {
        /// <summary>
        /// modem接收函数回调事件函数
        /// </summary>
        public event CommunDataReceiveHandler ModemDataReceiveHandler = null;
        /// <summary>
        /// modem管理线程
        /// </summary>
        private Thread ModemServerThread = null;
        /// <summary>
        /// 串口定义类
        /// </summary>
        private SerialServer ModemSerialServer = null;

        public string Name
        {
            get { return ModemSerialServer.Name; }
            set { ModemSerialServer.Name = value; }
        }

        public int Baudrate
        {
            get { return ModemSerialServer.BaudRate; }
            set { ModemSerialServer.BaudRate = value; }
        }

        public ModemServer()
        {
            ModemSerialServer = new SerialServer();
            ModemSerialServer.SerialDataReceiveComplated += new CommunDataReceiveHandler(OnModemDataReceiveHandler);
        }

        private bool _isStart = false;
        public bool IsStart
        {
            get { return _isStart; }
        }

        public ModemServer(string name, int baudrate)
        {
            ModemSerialServer = new SerialServer(name, baudrate);
            ModemSerialServer.SerialDataReceiveComplated += new CommunDataReceiveHandler(OnModemDataReceiveHandler);
        }

        public bool Start()
        {
            if (true == _isStart)
            {
                return false;
            }

            try
            {
                if (true == ModemSerialServer.Start())
                {
                    _isStart = true;
                    ModemServerThread = new Thread(OnMangerModemThreadRoutine);
                    ModemServerThread.Start();
                }
            }
            catch (Exception r)
            {
                _isStart = false;
                Console.WriteLine(r.Message);
            }

            return _isStart;
        }

        public bool Stop()
        {
            if (false == _isStart)
            {
                return false;
            }

            try
            {
                if (true == ModemSerialServer.Stop())
                {
                    _isStart = false;
                    ModemServerThread.Abort();
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return _isStart;
        }

        private void OnModemDataReceiveHandler(object sender, CommuEventArgs e)
        {
        }

        private void OnMangerModemThreadRoutine()
        {
            while (_isStart)
            {
                Thread.Sleep(200); //100ms
            }
        }

        /// <summary>
        ///  执行命令(允许重复)直到成功或者超时才会返回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="possibleAck"></param>
        /// <param name="timeout"></param>
        /// <param name="tries"></param>
        /// <returns></returns>
        private int ExecuteCommand(string command, string[] possibleAck, int timeout, int tries)
        {
            return 0;
        }

        public void Send(byte[] data, string telphone)
        {
        }

        public void Send(string data, string telphone)
        {
        }
    }
}

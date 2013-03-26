using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nms_comm_lib
{
    class AtCommandOfInit
    {
        public string Name;      // AT命令名称
        public string Ok;        // AT命令执行成功的响应关键字(为空表示不判断应答信息)
        public ushort Timeout;   // AT命令应答超时时间，单位：ms

        public AtCommandOfInit(string name, string ok, ushort timeout)
        {
            this.Name = name + string.Format("\r\n");
            this.Ok = ok;
            this.Timeout = timeout;
        }
    }

    class Commands
    {
        public static readonly AtCommandOfInit[] AtCommandsOfInitCdma;
        public static readonly AtCommandOfInit[] AtCommandsOfInitGsm;
        public static readonly AtCommandOfInit[] AtCommandsOfInitSms;
        static Commands()
        {
            // CDMA Modem的初始化指令集合
            AtCommandsOfInitCdma = new AtCommandOfInit[] {
                new AtCommandOfInit("AT", "OK", 2000), 
                new AtCommandOfInit("ATE0", "OK", 2000), 
                new AtCommandOfInit("ATS0=1", "OK", 2000), 
                new AtCommandOfInit("AT+CMGF=1", "OK", 2000), 
                new AtCommandOfInit("AT+CNMI=2,2,0,0,0", "OK", 2000), 
                new AtCommandOfInit("AT+WSCL=1,2", "OK", 2000), 
                new AtCommandOfInit("AT+CLIP=1", "OK", 2000), 
                new AtCommandOfInit("AT+CMGD=1,4", string.Empty, 2000), 
            };

            // GSM Modem的初始化指令集合
            AtCommandsOfInitGsm = new AtCommandOfInit[] {
                new AtCommandOfInit("AT", "OK", 2000), 
                new AtCommandOfInit("ATE0", "OK", 2000), 
                new AtCommandOfInit("ATS0=1", "OK", 2000), 
                new AtCommandOfInit("AT+CMGF=1", "OK", 2000),
                new AtCommandOfInit("AT+CNMI=2,2,0,0,0", "OK", 2000), 
                new AtCommandOfInit("AT+CSMP=21,0,0,0", "OK", 2000), 
                new AtCommandOfInit("AT+CLIP=1", "OK", 2000), 
                new AtCommandOfInit("AT+CMGD=1,4", string.Empty, 2000), 
            };

            // modem状态检测
            AtCommandsOfInitSms = new AtCommandOfInit[] {
                new AtCommandOfInit("AT+CMGF?","+CMGF: 1", 2000),
                new AtCommandOfInit("AT+CNMI?","+CNMI: 2,2,0,1,0", 2000),
                new AtCommandOfInit("AT+CMGD=1,4","OK", 2000),
            };
        }
    }

    public enum SmsTxRx
    {
        None = 0x00, TX, RX,
    }

    class SmsMode
    {
        bool _isStart = false;
        bool _isSmsStatus = false;

        string smsMessageBody = string.Empty;

        SerialMode serialMode = null;
        Thread smsThread = null;
        List<byte[]> commReceived = new List<byte[]>();
        public event CommunDataReceiveHandler ModemDataRecevieCompleted = null;
        public event CommunDataReceiveHandler ModemLogsRecevieCompleted = null;

        private SmsTxRx txrx = SmsTxRx.TX;
        public SmsTxRx TxRx
        {
            get { return txrx; }
            set { txrx = value; }
        }

        public string Name
        {
            get { return serialMode.Name; }
            set { serialMode.Name = value; }
        }

        public int Baudrate
        {
            get { return serialMode.BaudRate; }
            set { serialMode.BaudRate = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SmsMode()
        {
            serialMode = new SerialMode();
            serialMode.SerialDataReceiveComplated += new CommunDataReceiveHandler(OnDataReceiveFromHandle);
            serialMode.SerialLogsReceiveComplated += new CommunDataReceiveHandler(OnCommunLogsReceiveComplated);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="baudrate"></param>
        public SmsMode(string name, int baudrate, int timeout, SmsTxRx txrx)
        {
            TxRx = txrx;
            serialMode = new SerialMode(name, baudrate, timeout);
            serialMode.SerialDataReceiveComplated += new CommunDataReceiveHandler(OnDataReceiveFromHandle);
            serialMode.SerialLogsReceiveComplated += new CommunDataReceiveHandler(OnCommunLogsReceiveComplated);
        }

        ~SmsMode()
        {
            if (serialMode.IsStart)
            {
                serialMode.Stop();
                serialMode.SerialDataReceiveComplated -= new CommunDataReceiveHandler(OnDataReceiveFromHandle);
                serialMode.SerialLogsReceiveComplated -= new CommunDataReceiveHandler(OnCommunLogsReceiveComplated);
            }
        }

        /// <summary>
        /// 字符串发送函数
        /// </summary>
        /// <param name="message"></param>
        private void SendTo(string message)
        {
            try
            {
                serialMode.Send(message);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        /// <summary>
        /// 字节流发送函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        private void SendTo(byte[] message, int length)
        {
            try
            {
                byte[] sms = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    sms[i] = message[i];
                }

                SendTo(Encoding.ASCII.GetString(sms));
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }
        /// <summary>
        /// 短信发送函数
        /// </summary>
        /// <param name="telphone"></param>
        /// <param name="message"></param>
        public void SendToSMS(string telphone, string message)
        {
            try
            {
                lock (smsMessageBody)
                {
                    SendTo(telphone);
                    smsMessageBody = message;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }
        /// <summary>
        /// 短信发送函数
        /// </summary>
        /// <param name="telphone"></param>
        /// <param name="message"></param>
        /// <param name="length"></param>
        public void SendToSMS(string telphone, byte[] message, int length)
        {
            try
            {
                byte[] sms = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    sms[i] = message[i];
                }

                string _message = Encoding.ASCII.GetString(sms);

                lock (smsMessageBody)
                {
                    SendTo(telphone);
                    smsMessageBody = _message;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private string smsAtCommandCMGD = string.Empty;
        /// <summary>
        /// modem短信接收函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceiveFromHandle(object sender, CommuEventArgs e)
        {          
            string sms = Encoding.ASCII.GetString(e.Data).Replace('\0', '?').ToUpper();

            // 发送短消息
            if ((sms.IndexOf('>') >= 0) && (sms.Trim().Length == 1))
            {
                lock (smsMessageBody)
                {
                    if (!string.IsNullOrEmpty(smsMessageBody))
                    {
                        smsMessageBody = string.Format("{0}{1}", smsMessageBody, 0x1A);
                        SendTo(smsMessageBody);
                    }

                    smsMessageBody = string.Empty;
                }

                return;
            }

            // 收到新的短消息
            if (sms.Contains("+CMTI") && sms.Contains("SM"))
            {
                int index = 0; // 短消息位置
                string[] subs = sms.Split(',');

                if (int.TryParse(subs[subs.Length - 1], out index))
                {
                    // 发送读取短信
                    SendTo(string.Format("AT+CMGR={0}\r\n", index));
                    smsAtCommandCMGD = string.Format("AT+CMGD={0}\r\n", index);
                }

                return;
            }

            // 读到短消息之后删除该条短消息
            if (sms.Contains("+CMGR"))
            {
                if (!string.IsNullOrEmpty(smsAtCommandCMGD))
                {
                    SendTo(smsAtCommandCMGD);
                    smsAtCommandCMGD = string.Empty;
                }
                //  将收到的数据传到上传
                if (null != ModemDataRecevieCompleted)
                {
                    CommuEventArgs args = new CommuEventArgs(e.Data, CommunicateMode.SMS);
                    ModemDataRecevieCompleted(sender, args);
                }

                return;
            }

            lock (commReceived)
            {
                commReceived.Add(e.Data);
            }
        }
        /// <summary>
        /// 开始，调用此函数后会起一个线程来管理modem初始化和每间隔一段时间检测modem的状态
        /// </summary>
        /// <param name="tries"></param>
        /// <returns>打开串口和线程建立成功返回true，否则返回false</returns>
        public bool Start(int tries)
        {
            try
            {
                if (serialMode.IsStart)
                {
                    return false;
                }

                _isStart = true;
                serialMode.Start();
                //建立modem管理线程
                smsThread = new Thread(SmsThreadRoutine);
                smsThread.Start(tries); // 设置modem状态监测次数
            }
            catch (Exception r)
            {
                _isStart = false;
                Console.WriteLine(r.Message);
            }

            return _isStart;
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns>正常停止返回true，否则返回false</returns>
        public bool Stop()
        {
            try
            {
                if (serialMode.IsStart == false)
                {
                    return false;
                }

                _isStart = false;
                serialMode.Stop();
                smsThread.Abort();
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return !_isStart;
        }
        /// <summary>
        /// modem线程管理回调函数，此函数一直运行知道结束
        /// </summary>
        /// <param name="state"></param>
        private void SmsThreadRoutine(object state)
        {
            int count = 0;
            int tries = (int)state;

            _isSmsStatus = false;

            while (_isStart)
            {
                try
                {
                    if (_isSmsStatus == false)
                    {
                        // 检测Modem的类型
                        int modemSystem = ExecuteCommand("AT+CSCS=?\r\n", new string[] { "CDMA", "GSM" }, 5000, tries);                      

                        AtCommandOfInit[] commands = new AtCommandOfInit[0];

                        switch (modemSystem)
                        {
                            case 0: // CDMA modem
                                commands = Commands.AtCommandsOfInitCdma;
                                break;

                            case 1: //GSM modem
                                commands = Commands.AtCommandsOfInitGsm;
                                break;

                            default:
                                break;
                        }

                        bool bFlags = true;
                        int commandResult = -1;

                        foreach (AtCommandOfInit command in commands)
                        {
                            if (string.IsNullOrEmpty(command.Ok))
                            {
                                // 仅仅发送命令，不检查执行结果
                                commandResult = ExecuteCommand(command.Name, new string[0], command.Timeout, tries);
                            }
                            else
                            {
                                // 发送命令，并判断命令执行是否成功
                                commandResult = ExecuteCommand(command.Name, new string[] { command.Ok }, command.Timeout, tries);
                            }

                            // 命令执行不成功
                            if (commandResult < 0 && count < 5)
                            {
                                bFlags = false;
                                // 如果command执行错误则调跳出重新执行，直到执行次数超过5次
                                _isSmsStatus = false;
                                break;
                            }
                        }

                        if (bFlags)
                        {
                            _isSmsStatus = true;
                        }
                        else
                        {
                            count++;
                        }
                        Thread.Sleep(30000); // 等待6秒
                    }
                    else // 如果状态为false则表示modem初始化成功，则需要等待一段时间后再检测modem的状态
                    {
                        count = 0;
                        Thread.Sleep(60000); // 等待60秒
                        // 当返回为true时表示modem的状态丢失，需要重新初始化modem指令
                        _isSmsStatus = SmsStatusCheck(tries);
                    }
                }
                catch (Exception r)
                {
                    Console.WriteLine(r.Message);
                }
            }
        }
        /// <summary>
        /// modem状态管理函数，主要发送AT指令，检测AT指令返回值，从而来检查modem状态
        /// </summary>
        /// <param name="tries"></param>
        /// <returns>modem状态正常返回true，不正常返回false</returns>
        private bool SmsStatusCheck(int tries)
        {
            int commandResult = -1;
            AtCommandOfInit[] commands = Commands.AtCommandsOfInitSms;

            foreach (AtCommandOfInit command in commands)
            {
                if (string.IsNullOrEmpty(command.Ok))
                {
                    // 仅仅发送命令，不检查执行结果
                    commandResult = ExecuteCommand(command.Name, new string[0], command.Timeout, tries);
                }
                else
                {
                    // 发送命令，并判断命令执行是否成功
                    commandResult = ExecuteCommand(command.Name, new string[] { command.Ok }, command.Timeout, tries);
                }

                // 命令执行不成功
                if (commandResult < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 执行命令(允许重复)直到成功或者超时才会返回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="possibleAck"></param>
        /// <param name="timeout"></param>
        /// <param name="tries"></param>
        /// <returns>返回回应码数字中的序号值</returns>
        private int ExecuteCommand(string command, string[] possibleAck, int timeout, int tries)
        {
            while (tries-- > 0)
            {
                // 添加到发送队列，由发送定时器处理发送任务
                SendTo(command);

                // 对于不判断应答消息的命令进行处理
                if ((possibleAck == null) || (possibleAck.Length == 0))
                {
                    Thread.Sleep(timeout);
                    return 0;   // 直接返回0代表成功
                }

                // 每隔100ms检查串口是否有应答
                // sleeps为超时时间对应的检查次数
                int sleeps = (timeout + 99) / 100;

                while (sleeps-- > 0)
                {
                    Thread.Sleep(100); // 每隔100ms检查一次接收队列

                    lock (commReceived)
                    {
                        // 依次分析串口接收到的数据包
                        foreach (byte[] element in commReceived)
                        {
                            string _at = Encoding.ASCII.GetString(element).Replace('\0', '?');

                            for (int n = 0; n < possibleAck.Length; n++)
                            {
                                if (_at.IndexOf(possibleAck[n]) >= 0)
                                {
                                    commReceived.Remove(element);
                                    return n; // 收到串口应答，并返回应答编号
                                }
                            }
                        }
                    }
                }
            }

            return -1; // 命令执行不成功
        }

        private void OnCommunLogsReceiveComplated(object sender, CommuEventArgs e)
        {
            if (null != ModemLogsRecevieCompleted)
            {
                e.Mode = CommunicateMode.SMS;                
                ModemLogsRecevieCompleted(sender, e);
            }
        }
    }
}

using nms_usercontrol_libs.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using nms_comm_lib;
using nms_database_lib;
using Microsoft.Win32;
using nms_excel_lib;
using System.Data;
using CmccLib;

namespace omc.src
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        // 中国移动协议包序列号累加变量
        private ushort cmccPacketNo = 0x0000;

        CommunicateBase communicateBase = new CommunicateBase();
        MoidListBusiness moidListBusiness = new MoidListBusiness();

        public Main()
        {
            InitializeComponent();

            communicateBase = new CommunicateBase();
            communicateBase.CommunDataReceiveComplated += new CommunDataReceiveHandler(DefaultReceiveDataResponseHandler);

            ToolBarClickHandlerInitial();            
        }

        private void ToolBarClickHandlerInitial()
        {
            ToolBar.ButtonExitClickEvent += new UserLoginClickEventHandler(ButtonExitEventHandler);
            ToolBar.BtnUserManageClickEvent += new ButtonClickEventHandler(BtnUserManageEventHandler);
            ToolBar.BtnPortSettingClickEvent += new ButtonClickEventHandler(BtnPortSettingEventHandler);
        }

        /**************************************************
         * 退出应用程序处理函数
         * ***********************************************/
        private void ButtonExitEventHandler(object sender, UserEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TopMenuExitClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit application?", "Exit Applicatoin", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }        
        }

        /*************************************************************************
         * 用户管理窗体
         * **********************************************************************/
        private void BtnUserManageEventHandler(object sender, EventArgs e)
        {
            UserManageWin win = new UserManageWin();
            win.ShowDialog();
        }

        /*************************************************************************
         * 端口设置窗体
         * **********************************************************************/
        private void BtnPortSettingEventHandler(object sender, EventArgs e)
        {
            PortSettingWin win = new PortSettingWin();
            win.ShowDialog();
        }

        #region Data send to monitor device and form device receive data
        /// <summary>
        /// 数据发送函数，此函数为主界面回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataGridOperationHandler(object sender, DataGridEventArgs e)
        {
            try
            {
                if (e.SiteList == null)
                {
                    MessageBox.Show("Please select item try again", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (DataGridOperMode.None == e.Mode || DataGridOperMode.Unknow == e.Mode)
                {
                    MessageBox.Show("Please select valid operation try again", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<Cmcc.MonitorObject> moidList = new List<Cmcc.MonitorObject>();
                foreach (SiteModel element in e.SiteList)
                {
                    Cmcc.MonitorObject mo = new Cmcc.MonitorObject();
                    mo.Moid = element.Moid;
                    mo.Data = new byte[element.Length];

                    //如果是设置命令则需要复制数据到moid对象中，以便将数据设置到设备
                    if (e.Mode == DataGridOperMode.Setting)
                    {
                        for (int n = 0; n < element.Length; n++)
                        {
                            mo.Data[n] = element.Data[n];
                        }
                    }

                    moidList.Add(mo);
                }

                byte command = 0;

                // 操作命令码设置
                switch (e.Mode)
                {
                    case DataGridOperMode.Query:
                        command = Cmcc.CmccConst.Command.Query;
                        break;

                    case DataGridOperMode.Setting:
                        command = Cmcc.CmccConst.Command.Setting;
                        break;

                    case DataGridOperMode.None:
                    case DataGridOperMode.Unknow:
                        MessageBox.Show("Command code error,please try again", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    default:
                        MessageBox.Show("Command code error,please try again", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                Cmcc.APLayer apLayer;
                Cmcc.NPLayer npLayer;
                Cmcc.MapLayer mapLayer;

                Business business = new Business();
                business.CmccClassInformationInit(0x88888, 0x00, command, out apLayer, out npLayer, out mapLayer);
                mapLayer.objects = moidList.ToArray();

                byte[] data = Cmcc.CmccModule.Packet(apLayer, npLayer, mapLayer);
                if (null == data)
                {
                    MessageBox.Show("Encode cmcc packet error,please try again", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 数据接收函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultReceiveDataResponseHandler(object sender, CommuEventArgs e)
        {
            try
            {
                string strPhoneText = string.Empty;

                //  如果通信方式为SMS则取出电话号码
                if (e.Mode == CommunicateMode.SMS)
                {
                    string strText = Encoding.ASCII.GetString(e.Data);
                    //取收到消息中的手机号码
                    if (strText.Contains("+CMT:"))
                    {
                        string phoneText = strText.Substring(0, strText.IndexOf(","));
                        int phoneStart = phoneText.IndexOf("\"");
                        int phoneEnd = phoneText.LastIndexOf("\"");
                        if (phoneStart > 0 && phoneEnd > 0)
                        {
                            strPhoneText = phoneText.Substring(phoneStart + 1, phoneEnd - phoneStart - 1);
                        }
                    }
                }

                Cmcc.APLayer apLayer = new Cmcc.APLayer();
                Cmcc.NPLayer npLayer = new Cmcc.NPLayer();
                Cmcc.MapLayer mapLayer = new Cmcc.MapLayer();
                // 解释协议数据包并处理
                Cmcc.CmccUnpacketResult UnpacketResult = Cmcc.CmccModule.Unpacket(e.Data, ref apLayer, ref npLayer, ref mapLayer);
                if (UnpacketResult != Cmcc.CmccUnpacketResult.Succeed)
                {
                    // 提示解析数据错误
                    return;
                }

                // 如果是告警上报则保持告警后回复
                if (mapLayer.commandId == Cmcc.CmccConst.Command.Report)
                {
                    // 保存并显示告警
                    Business alarmBusiness = new Business();
                    alarmBusiness.AlarmInformationProcess(npLayer, mapLayer);

                    // 回复下位机
                    CommuEventArgs args = null;
                    npLayer.npFlag = Cmcc.CmccConst.AckFlag.Succeed;
                    mapLayer.ackFlag = Cmcc.CmccConst.AckFlag.Succeed;
                    byte[] data = Cmcc.CmccModule.Packet(apLayer, npLayer, mapLayer);
                    if (null == data) return;

                    switch (e.Mode)
                    {
                        case CommunicateMode.GPRS:
                            args = new CommuEventArgs(data, e.GprsObject);
                            break;

                        case CommunicateMode.CSD:
                            /* nothing ... */
                            break;

                        case CommunicateMode.RS232:
                            args = new CommuEventArgs(data, CommunicateMode.RS232);
                            break;

                        case CommunicateMode.SMS:
                            args = new CommuEventArgs(data, strPhoneText);
                            break;

                        case CommunicateMode.SNMP:
                            /* nothing ... no support mode */
                            break;
                        case CommunicateMode.TCP:
                            args = new CommuEventArgs(data, e.GprsObject);
                            break;

                        case CommunicateMode.UDP:
                            args = new CommuEventArgs(data, e.EndPoint);
                            break;

                        default:
                            args = null;
                            break;
                    }

                    // 如果args为空则不知道上报的通信方式，放弃此数据包的回复
                    if (null == args) return;
                    // 回复上报告警的设备
                    communicateBase.Send(data, args);
                    return;  // 告警上报处理完成狗直接返回
                }


                // 处理获取直放站监控列表流程处理
                if (mapLayer.commandId == Cmcc.CmccConst.Command.Query)
                {
                    Cmcc.MonitorObject mo = mapLayer.objects[0];
                   
                    if (mapLayer.objects.Length == 1 && mo.Moid == moidListBusiness.MoidList)
                    {
                        // 获得总包数和包序号
                        byte total = mo.Data[0];  // 总包数
                        byte index = mo.Data[1];  // 包序号
                        moidListBusiness.SetTotalBusiness(total, index);

                        // 处理接收到的数据包
                        Business business = new Business();
                        business.ProcessMoidListForCmccResponse(RepeaterParaDataGrid, mapLayer);

                        if (moidListBusiness.IsHas == true)
                        {
                            // 发送下一包数据
                        }
                        else // 读取设备类型数据包
                        {

                        }

                        return;
                    }

                    if (mapLayer.objects.Length == 1 && moidListBusiness.MoidType == mo.Moid)
                    {
                        // 处理系统类型数据
                    }
                }

                // 处理查找或设置返回的数据包
                if (mapLayer.commandId == Cmcc.CmccConst.Command.Query || mapLayer.commandId == Cmcc.CmccConst.Command.Setting)
                {
                    Business paramBusiness = new Business();
                    List<SiteModel> siteModelList = paramBusiness.RepeaterParamInformationProcess(npLayer, mapLayer);

                    // 跨线程调用
                    DataGridEventArgs args = new DataGridEventArgs(siteModelList, mapLayer.commandId == Cmcc.CmccConst.Command.Query ? DataGridOperMode.Query : DataGridOperMode.Setting);
                    this.RepeaterParaDataGrid.Dispatcher.Invoke(new DataGridOperationHandler(DefaultReceiveDataProcessHandler), RepeaterParaDataGrid, args);
                }
            }
            catch (Exception r)
            {
                MessageBox.Show(r.Message, "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        /// <summary>
        /// 将数据传到DataGrid控件中进行处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultReceiveDataProcessHandler(object sender, DataGridEventArgs e)
        {
            this.RepeaterParaDataGrid.RepeaterParamProcessForUI(sender, e);
        }
        #endregion
    }
}

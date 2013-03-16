using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nms_usercontrol_libs.src;
using nms_comm_lib;
using CmccLib;
using nms_database_lib;

namespace omc
{
    class Business
    {
        /// <summary>
        /// 在列表中查询对应的moid对象
        /// </summary>
        /// <param name="moidList"></param>
        /// <param name="moid"></param>
        /// <returns></returns>
        public Moid FindMoidFormList(List<Moid> moidList, ushort moid)
        {
            foreach (Moid element in moidList)
            {
                if (element.Oid == moid)
                {
                    return element;
                }
            }

            return null;
        }
        /// <summary>
        /// 处理告警
        /// </summary>
        /// <param name="npLayer"></param>
        /// <param name="mapLayer"></param>
        public void AlarmInformationProcess(Cmcc.NPLayer npLayer, Cmcc.MapLayer mapLayer)
        {
            if (null == mapLayer)
            {
                return;
            }
            //根据站点编号和设备编号获取站点名称
            string siteName = string.Empty;
            Site site = Database.GetSiteBySiteId(npLayer.siteId, npLayer.subId);
            if (null != site)
            {
                siteName = site.SiteName;
            }
            // 获取监控对象列表
            List<Moid> moidList = Database.GetMoidAll();
            
            // 处理上报对象将对象保存到数据库中
            foreach (Cmcc.MonitorObject element in mapLayer.objects)
            {
                Moid moid = FindMoidFormList(moidList, element.Moid);
                if (null == moid) continue;
                // 产生告警对象并保存到数据库中作为历史告警
                Alarm alrm = new Alarm(npLayer.siteId.ToString(), npLayer.subId.ToString(), moid.Enname, DateTime.Now.ToString(), moid.Level.ToString(), siteName);
                Database.InsertAlarm(alrm);

                // 产生告警日志并保存到数据库-------------------------------------------
            }
        }
        /// <summary>
        /// 处理直放站设备设置和查询，将中国移动协议数据包内容转换为SiteModel界面显示的数据
        /// </summary>
        /// <param name="npLayer"></param>
        /// <param name="mapLayer"></param>
        /// <returns></returns>
        public List<SiteModel> RepeaterParamInformationProcess(Cmcc.NPLayer npLayer, Cmcc.MapLayer mapLayer)
        {
            List<SiteModel> siteModelList = new List<SiteModel>();

            foreach (Cmcc.MonitorObject element in mapLayer.objects)
            {
                SiteModel siteModel = new SiteModel(element.Moid, element.Length, element.Data);
                siteModelList.Add(siteModel);
            }

            return siteModelList;
        }

        /// <summary>
        /// 初始化协议类
        /// </summary>
        /// <param name="siteid"></param>
        /// <param name="subid"></param>
        /// <param name="command"></param>
        /// <param name="npFlag"></param>
        /// <param name="apLayer"></param>
        /// <param name="npLayer"></param>
        /// <param name="mapLayer"></param>
        public void CmccClassInformationInit(uint siteid, byte subid, byte command, out Cmcc.APLayer apLayer, out Cmcc.NPLayer npLayer, out Cmcc.MapLayer mapLayer)
        {
            apLayer = new Cmcc.APLayer();
            npLayer = new Cmcc.NPLayer();
            mapLayer = new Cmcc.MapLayer();

            apLayer.apId = Cmcc.CmccConst.ApId.A;
            npLayer.subId = subid;
            npLayer.siteId = siteid;
            npLayer.npId = Cmcc.CmccConst.NpId.A;
            npLayer.npFlag = Cmcc.CmccConst.NpFlag.Command;

            mapLayer.commandId = command;
            mapLayer.mapId = Cmcc.CmccConst.MapId.A;
            mapLayer.ackFlag = Cmcc.CmccConst.AckFlag.Command;
        }

        /// <summary>
        /// 返回获取moid列表的数据包
        /// </summary>
        /// <param name="packno"></param>
        /// <param name="total"></param>
        /// <param name="index"></param>
        /// <param name="apLayer"></param>
        /// <param name="npLayer"></param>
        /// <param name="mapLayer"></param>
        /// <returns></returns>
        public byte[] GetMoidListPacket(ushort packno, byte total, byte index, Cmcc.APLayer apLayer, Cmcc.NPLayer npLayer, Cmcc.MapLayer mapLayer)
        {
            MoidListBusiness business = new MoidListBusiness();

            Cmcc.MonitorObject mo = new Cmcc.MonitorObject();
            mo.Moid = business.MoidList;
            mo.Data = new byte[2];
            mo.Data[0] = total;
            mo.Data[1] = index;

            List<Cmcc.MonitorObject> objects = new List<Cmcc.MonitorObject>();
            objects.Add(mo);

            npLayer.packetId = packno;
            mapLayer.objects = objects.ToArray();

            return Cmcc.CmccModule.Packet(apLayer, npLayer, mapLayer);
        }
        /// <summary>
        /// 获得设备类型数据包
        /// </summary>
        /// <param name="nPacketNo"></param>
        /// <param name="apLayer"></param>
        /// <param name="npLayer"></param>
        /// <param name="mapLayer"></param>
        /// <returns></returns>
        public byte[] GetDeviceTypePacket(ushort nPacketNo,Cmcc.APLayer apLayer, Cmcc.NPLayer npLayer, Cmcc.MapLayer mapLayer)
        {
            MoidListBusiness business = new MoidListBusiness();

            Cmcc.MonitorObject mo = new Cmcc.MonitorObject();
            mo.Moid = business.MoidType;
            mo.Data = new byte[1];
         
            List<Cmcc.MonitorObject> objects = new List<Cmcc.MonitorObject>();
            objects.Add(mo);

            npLayer.packetId = nPacketNo;
            mapLayer.objects = objects.ToArray();

            return Cmcc.CmccModule.Packet(apLayer, npLayer, mapLayer);
        }
        /// <summary>
        /// 计算中国移动协议包序列号，如果大于最大值则调整到最小值，否则加1返回
        /// </summary>
        /// <param name="nPackNo"></param>
        /// <returns>包序列号</returns>
        public ushort CmccPacketNumberInc(ushort nPackNo)
        {
            if (nPackNo >= Cmcc.CmccConst.PacketId.Max)
            {
                nPackNo = Cmcc.CmccConst.PacketId.Min;
            }

            nPackNo++;
            return nPackNo;
        }
        /// <summary>
        /// 将数据传到界面线程中去显示
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="siteModelList"></param>
        private delegate void ThreadProcessMoidListForCmccResponseHandler(RepeaterParaTabs tabs, List<SiteModel> siteModelList);
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="siteModelList"></param>
        public void ThreadProcessMoidListForCmccResponse(RepeaterParaTabs tabs, List<SiteModel> siteModelList)
        {
            tabs.AddRepeaterMonitorParamForUI(siteModelList);
        }
        /// <summary>
        /// 处理接收监控参量列表的数据包
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="mapLayer"></param>
        public void ProcessMoidListForCmccResponse(RepeaterParaTabs tabs, Cmcc.MapLayer mapLayer)
        {
            if (null == mapLayer)
            {
                return;
            }

            List<Moid> moidList = Database.GetMoidAll();
            List<SiteModel> siteModelList = new List<SiteModel>();

            foreach (Cmcc.MonitorObject element in mapLayer.objects)
            {               
                // 最前面两哦字节为总包数和序号
                for (int n = 0; n < element.Length - 2; n += 2)
                {
                    ushort oid = BitConverter.ToUInt16(element.Data, n + 2);
                    Moid moid = FindMoidFormList(moidList, oid);
                    if (null == moid)
                    {
                        continue;
                    }

                    SiteModel siteModel = new SiteModel(moid.ID, moid.Readonly, moid.Oid, moid.Length, moid.Enname, moid.Unit, moid.Type);
                    siteModelList.Add(siteModel);
                }                
            }

            if (siteModelList.Count > 0)
            {
                tabs.Dispatcher.Invoke(new ThreadProcessMoidListForCmccResponseHandler(ThreadProcessMoidListForCmccResponse), tabs, siteModelList);
            }
        }

        /// <summary>
        /// 处理接收监控参量列表的数据包
        /// </summary>
        /// <param name="tabs"></param>
        /// <param name="mapLayer"></param>
        public void ProcessQuerySettingForCmccResponse(RepeaterParaTabs tabs, Cmcc.MapLayer mapLayer)
        {
        }
    }

    public class MoidListBusiness
    {
        private ushort _moidList = 0x0009;
        public ushort MoidList
        {
            get { return _moidList; }
        }

        private ushort _moidType = 0x0003;
        public ushort MoidType
        {
            get { return _moidType; }
        }

        private byte _total = 0;
        public byte Total
        {
            get { return _total; }
            set { _total = value; }
        }

        private byte _index = 0;
        public byte Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public bool IsHas
        {
            get { return _total > _index; }
        }

        public MoidListBusiness()
        {
            _total = 0;
            _index = 0;
        }

        public MoidListBusiness(byte total, byte index)
        {
            _total = total;
            _index = index;
        }

        public void SetTotalBusiness(byte total, byte index)
        {
            _total = total;
            _index = index;
        }
    }
}

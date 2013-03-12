using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_database_lib
{
    #region Moid object database table
    enum MoidColumn
    {
        ID = 0x00, Moid, Enname, Cnname, Readonly, Type, AlarmLevel, Length, Value, Unit, SystemType,
    }
    /// <summary>
    /// moid静态管理表，符合中国移动协议的所有监控对象标识和数据定义
    /// </summary>
    public class Moid
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public int Length { get; set; }
        public bool Readonly { get; set; }
        public ushort Oid { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public string Enname { get; set; }
        public string Cnname { get; set; }
        public string Value { get; set; }
        public string System { get; set; }

        public Moid()
        {
        }

        /// <summary>
        /// 类构造函数，带所有参量,其不带参数的构造函数为默认的
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="length"></param>
        /// <param name="only"></param>
        /// <param name="moid"></param>
        /// <param name="unit"></param>
        /// <param name="type"></param>
        /// <param name="enname"></param>
        /// <param name="cnname"></param>
        /// <param name="value"></param>
        /// <param name="system"></param>
        public Moid(int id, int level, int length, bool only, ushort oid, string unit, string type, string enname, string cnname, string value, string system)
        {
            this.ID = id;
            this.Level = level;
            this.Length = length;
            this.Readonly = only;
            this.Oid = oid;
            this.Unit = unit;
            this.Type = type;
            this.Enname = enname;
            this.Cnname = cnname;
            this.Value = value;
            this.System = system;
        }

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <returns>返回一个新对象，但值和本实例的值相同</returns>
        public Moid Clone()
        {
            return new Moid(this.ID, this.Level, this.Length, this.Readonly, this.Oid, this.Unit, this.Type, this.Enname, this.Cnname, this.Value, this.System);
        }
    }
    #endregion

    #region Site object database table
    enum SiteColumn
    {
        ID = 0x00, PID, SiteId, SubId, DeviceType, Model, SiteType, CommMode, CreateDate, Telphone, sProtocol, SiteName, sAddress, sFactory, IPAddress, Port, Moidlist,
    }
    /// <summary>
    /// 直放站站点管理表
    /// </summary>
    public class Site
    {
        public int ID { get; set; }
        public int PID { get; set; }         // 左边树状图管理之用
        public uint SiteId { get; set; }     // 站点编号
        public byte SubId { get; set; }      // 设备编号
        public byte DeviceType { get; set; } // 设备类型
        public byte Model { get; set; }      // 通信方式
        public byte SiteType { get; set; }
        public byte CommMode { get; set; }
        public uint IPAddress { get; set; }
        public ushort Port { get; set; }
        public string Telphone { get; set; }
        public string sProtocol { get; set; }
        public string SiteName { get; set; }
        public string sAddress { get; set; }
        public string sFactory { get; set; }
        public string Moidlist { get; set; }   // 每个设备的MOID列表，采用直存储监控参量列表的形式保存每个设备的监控参量

        public Site()
        {
        }

        public Site(int id, int pid, uint siteid, byte subid, byte devicetype, byte model, uint ipaddress, ushort port, string telphone, string moidlist)
        {
            ID = id;
            PID = pid;
            SiteId = siteid;
            SubId = subid;
            DeviceType = devicetype;
            Model = model ;
            IPAddress = ipaddress;
            Port = port;
            Telphone = telphone;
            Moidlist = moidlist;
        }

        public Site Clone()
        {
            return new Site(this.ID, this.PID, this.SiteId, this.SubId, this.DeviceType, this.Model, this.IPAddress, this.Port, this.Telphone, this.Moidlist);
        }
    }
    #endregion

    #region user table object define
    enum UserColumn
    {
        ID = 0x00, GroupID, UserName, Password, Sex, Email, Telphone, CreateDate,
    }

    public class User
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Telphone { get; set; }
        public string CreateDate { get; set; }

        public User(int id, int groupID, string username, string password, string sex, string email, string telphone, string createdate)
        {
             this.ID = id;
             this.GroupID = groupID;
             this.UserName = username;
             this.Password = password;
             this.Sex = sex; 
             this.Email = email;
             this.Telphone = telphone;
             this.CreateDate = createdate;
        }

        public User Clone()
        {
            return new User(this.ID, this.GroupID, this.UserName, this.Password, this.Sex, this.Email, this.Telphone, this.CreateDate);
        }
    }
    #endregion

    #region timeout list object define
    enum TimeoutColumn
    {
        ID = 0x00, Timeout,
    }

    public class Timeout
    {
        public int TimeValue { get; set; }

        public Timeout(int time)
        {
            this.TimeValue = time;
        }

        public Timeout Clone()
        {
            return new Timeout(this.TimeValue);
        }
    }
    #endregion

    #region group list object define
    enum GroupColumn
    {
        ID = 0x00, GroupName, CreateDate,
    }

    public class Group
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public string CreateDate { get; set; }

        public Group(int id, string name, string createdate)
        {
            this.ID = id;
            this.GroupName = name;
            this.CreateDate = createdate;
        }

        public Group Clone()
        {
            return new Group(this.ID, this.GroupName, this.CreateDate);
        }
    }
    #endregion

    #region enum list object define
    enum EnumExColumn
    {
        ID = 0x00, sName, sKey, sValue,
    }

    public class EnumEx
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public EnumEx(int id, string name, string key, string value)
        {
            this.ID = id;
            this.Name = name;
            this.Key = key;
            this.Value = value;
        }

        public EnumEx Clone()
        {
            return new EnumEx(this.ID, this.Name, this.Key, this.Value);
        }
    }
    #endregion

    #region region list object define
    enum RegionColumn
    {
        ID = 0x00, RegionID, RegionName, CreateDate,
    }

    public class Region
    {
        public int ID { get; set; }
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public string CreateDate { get; set; }

        public Region(int id, int regionid, string regionname, string createdate)
        {
            this.ID = id;
            this.RegionID = regionid;
            this.RegionName = regionname;
            this.CreateDate = createdate;
        }

        public Region Clone()
        {
            return new Region(this.ID, this.RegionID, this.RegionName, this.CreateDate);
        }
    }
    #endregion

    #region alarm history object define
    enum AlarmColumn
    {
        ID = 0x00, SiteID, SubID, AlarmName, AlarmTime, AlarmLevel, SiteName,
    }

    public class Alarm
    {
        public int ID { get; set; }
        public string SiteId { get; set; }
        public string SubId { get; set; }
        public string AlarmName { get; set; }
        public string AlarmTime { get; set; }
        public string AlarmLevel { get; set; }
        public string SiteName { get; set; }

        public Alarm(int id, string siteid, string subid, string name, string time, string level, string sitename)
        {
            this.ID = id;
            this.SiteId = siteid;
            this.SubId = subid;
            this.AlarmName = name;
            this.AlarmTime = time;
            this.AlarmLevel = level;
            this.SiteName = sitename;
        }

        public Alarm Clone()
        {
            return new Alarm(this.ID, this.SiteId, this.SubId, this.AlarmName, this.AlarmTime, this.AlarmLevel, this.SiteName);
        }
    }
    #endregion

    #region alarm log object define
    enum AlarmLogColumn
    {
        ID = 0x00, SiteID, SubID, AlarmTime, AlarmIndex, Detail, Province, City, AltemNum, AlarmLevel,
    }

    public class AlarmLog
    {
        public int ID { get; set; }
        public uint SiteID { get; set; }
        public byte SubID { get; set; }
        public string AlarmTime { get; set; }
        public int AlarmIndex { get; set; }
        public string Detail { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public string AItemNum { get; set; }
        public int AlarmLevel { get; set; }

        public AlarmLog(int id, uint siteid, byte subid, string time, int index, string detail, int province, int city, string aitem, int level)
        {
            this.ID = id;
            this.SiteID = siteid;
            this.SubID = subid;
            this.AlarmTime = time;
            this.AlarmIndex = index;
            this.AlarmLevel = level;
            this.Detail = detail;
            this.Province = province;
            this.City = city;
            this.AItemNum = aitem;            
        }

        public AlarmLog Clone()
        {
            return new AlarmLog(this.ID, this.SiteID, this.SubID, this.AlarmTime, this.AlarmIndex, this.Detail, this.Province, this.City, this.AItemNum, this.AlarmLevel);
        }
    }
    #endregion

    #region operation log object define
    enum OpsLogColumn
    {
        ID = 0x00, OprUserID, OprTime, Detail, Province, City,
    }
    public class OpsLog
    {
        public int ID { get; set; }
        public int Province { get; set; }
        public int City { get; set; }
        public int OprUserID { get; set; }
        public string OprTime { get; set; }
        public string Detail { get; set; }

        public OpsLog(int id, int opruserid, int city, int province, string oprtime, string detail)
        {
            this.ID = id;
            this.OprUserID = opruserid;
            this.City = city;
            this.Province = province;
            this.OprTime = oprtime;
            this.Detail = detail;
        }

        public OpsLog Clone()
        {
            return new OpsLog(this.ID, this.OprUserID, this.City, this.Province, this.OprTime, this.Detail);
        }
    }
    #endregion

    #region factory information object define
    enum FactoryColumn
    {
        ID = 0x00, FactID, FactName, FactFlag, FactAddr, LinkMan, LinkTel, Email, Note,
    }

    public class Factory
    {
        public int ID { get; set; }
        public int FactID { get; set; }
        public string FactName { get; set; }
        public string FactFlag { get; set; }
        public string FactAddr { get; set; }
        public string LinkMan { get; set; }
        public string LinkTel { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }

        public Factory(int id, int factid, string name, string flag, string addr, string man, string tel, string email, string note)
        {
            this.ID = id;
            this.FactID = factid;
            this.FactName = name;
            this.FactFlag = flag;
            this.FactAddr = addr;
            this.LinkMan = man;
            this.LinkTel = tel;
            this.Email = email;
            this.Note = note;
        }

        public Factory Clone()
        {
            return new Factory(this.ID, this.FactID, this.FactName, this.FactFlag, this.FactAddr, this.LinkMan, this.LinkTel, this.Email, this.Note);
        }
    }
    #endregion

    #region base station information object define
    enum BaseStatColumn
    {
        ID = 0x00, BaseStatName, CID, X, Y, PNBCCH64, Province, City, BaseStatCode, Detail,
    }

    public class BaseStat
    {
        public int ID { get; set; }
        public int City { get; set; }
        public int Province { get; set; }
        public int BaseStatCode { get; set; }
        public string BaseStatName { get; set; }
        public string CID { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string PNBCCH64 { get; set; }
        public string Detail { get; set; }

        public BaseStat(int id, string name, string cid, string x, string y, string pnbcch, int province, int city, int code, string detail)
        {
            this.ID = id;
            this.City = city;
            this.Province = province;
            this.BaseStatCode = code;
            this.BaseStatName = name;
            this.CID = cid;
            this.X = x;
            this.Y = y;
            this.PNBCCH64 = pnbcch;
            this.Detail = detail;
        }

        public BaseStat Clone()
        {
            return new BaseStat(this.ID, this.BaseStatName, this.CID, this.X, this.Y, this.PNBCCH64, this.Province, this.City, this.BaseStatCode, this.Detail);
        }
    }
    #endregion
}

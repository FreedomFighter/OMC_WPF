using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_database_lib
{
    public class Database
    {
        #region moid object table
        public static Moid GetMoidByMoid(ushort moid)
        {
            MoidTable table = new MoidTable();
            return table.GetMoidByMoid(moid);
        }

        public static List<Moid> GetMoidAll()
        {
            MoidTable table = new MoidTable();
            return table.GetMoidAll();
        }
        #endregion

        #region site object table
        public static Site GetSiteBySiteId(uint siteid, byte subid)
        {
            SiteTable table = new SiteTable();
            return table.GetSiteBySiteId(siteid, subid);
        }

        public static List<Site> GetSiteAll()
        {
            SiteTable table = new SiteTable();
            return table.GetSiteAll();
        }

        public bool SiteUpdate(Site site)
        {
            SiteTable table = new SiteTable();
            return table.Update(site);
        }

        public bool SiteInsert(Site site)
        {
            SiteTable table = new SiteTable();
            return table.Insert(site);
        }
        #endregion

        #region user object table
        public static User GetUserByName(string username)
        {
            UserTable table = new UserTable();
            return table.GetUserByName(username);
        }

        public static List<User> GetUserAll()
        {
            UserTable table = new UserTable();
            return table.GetUserAll();
        }

        public static bool UpdateUser(User user)
        {
            UserTable table = new UserTable();
            return table.Update(user);
        }

        public static bool InsertUser(User user)
        {
            UserTable table = new UserTable();
            return table.Insert(user);
        }

        public static bool DeleteUser(User user)
        {
            UserTable table = new UserTable();
            return table.Delete(user);
        }
        #endregion

        #region timeout object table
        public static List<Timeout> GetTimeoutAll()
        {
            TimeoutTable table = new TimeoutTable();
            return table.GetTimeoutAll();
        }
        #endregion

        #region group object table operation function
        public static Group GetGroupByID(int id)
        {
            GroupTable table = new GroupTable();
            return table.GetGroupByID(id);
        }

        public static Group GetGroupByName(string groupname)
        {
            GroupTable table = new GroupTable();
            return table.GetGroupByName(groupname);
        }

        public static List<Group> GetGroupAll()
        {
            GroupTable table = new GroupTable();
            return table.GetGroupAll();
        }

        public static bool UpdateGroup(Group group)
        {
            GroupTable table = new GroupTable();
            return table.Update(group);
        }

        public static bool InsertGroup(Group group)
        {
            GroupTable table = new GroupTable();
            return table.Insert(group);
        }

        public static bool DeleteGroup(Group group)
        {
            GroupTable table = new GroupTable();
            return table.Delete(group);
        }
        #endregion

        #region enumex object table operation function
        public static List<EnumEx> GetEnumExAll()
        {
            EnumExTable table = new EnumExTable();
            return table.GetEnumExAll();
        }
        #endregion

        #region region object table operation function
        public static Region GetRegionByID(int id)
        {
            RegionTable table = new RegionTable();
            return table.GetRegionByID(id);
        }

        public static Region GetRegionByName(string name)
        {
            RegionTable table = new RegionTable();
            return table.GetRegionByName(name);
        }

        public static List<Region> GetRegionAll()
        {
            RegionTable table = new RegionTable();
            return table.GetRegionAll();
        }

        public static bool UpdateRegion(Region region)
        {
            RegionTable table = new RegionTable();
            return table.Update(region);
        }

        public static bool InsertRegion(Region region)
        {
            RegionTable table = new RegionTable();
            return table.Insert(region);
        }

        public static bool DeleteRegion(Region region)
        {
            RegionTable table = new RegionTable();
            return table.Delete(region);
        }
        #endregion

        #region alrm object table operation function
        public static Alarm GetAlarmByID(int id)
        {
            AlarmTable table = new AlarmTable();
            return table.GetAlarmByID(id);
        }              

        public static List<Alarm> GetAlarmAll()
        {
            AlarmTable table = new AlarmTable();
            return table.GetAlarmAll();
        }

        public static bool UpdateAlarm(Alarm alarm)
        {
            AlarmTable table = new AlarmTable();
            return table.Update(alarm);
        }

        public static bool InsertAlarm(Alarm alarm)
        {
            AlarmTable table = new AlarmTable();
            return table.Insert(alarm);
        }

        public static bool DeleteAlarm(Alarm alarm)
        {
            AlarmTable table = new AlarmTable();
            return table.Delete(alarm);
        }
        #endregion

        #region alarm log object define
        public static AlarmLog GetAlarmLogByID(int id)
        {
            AlarmLogTable table = new AlarmLogTable();
            return table.GetAlarmLogByID(id);
        }

        public static List<AlarmLog> GetAlarmLogAll()
        {
            AlarmLogTable table = new AlarmLogTable();
            return table.GetAlarmLogAll();
        }

        public static bool InsertAlarmLog(AlarmLog alarmLog)
        {
            AlarmLogTable table = new AlarmLogTable();
            return table.InsertAlarmLog(alarmLog);
        }

        public static bool DeleteAlarmLog(AlarmLog alarmLog)
        {
            AlarmLogTable table = new AlarmLogTable();
            return table.DeleteAlarmLog(alarmLog);  
        }
        #endregion

        #region operation log object define       
        public static OpsLog GetOpsLogByID(int id)
        {
            OpsLogTable table = new OpsLogTable();
            return table.GetOpsLogByID(id);
        }

        public static List<OpsLog> GetOpsLogAll()
        {
            OpsLogTable table = new OpsLogTable();
            return table.GetOpsLogAll();
        }

        public static bool InsertOpsLog(OpsLog oprLog)
        {
            OpsLogTable table = new OpsLogTable();
            return table.InsertOpsLog(oprLog);
        }

        public static bool DeleteOpsLog(OpsLog oprLog)
        {
            OpsLogTable table = new OpsLogTable();
            return table.DeleteOpsLog(oprLog);
        }
        #endregion

        #region factory information object define
        public static Factory GetFactoryByID(int id)
        {
            FactoryTable table = new FactoryTable();
            return table.GetFactoryByID(id);
        }

        public static List<Factory> GetFactoryAll()
        {
            FactoryTable table = new FactoryTable();
            return table.GetFactoryAll();
        }

        public static bool UpdateFactory(Factory factory)
        {
            FactoryTable table = new FactoryTable();
            return table.Update(factory);
        }

        public static bool InsertFactory(Factory factory)
        {
            FactoryTable table = new FactoryTable();
            return table.Insert(factory);
        }

        public static bool DeleteFactory(Factory factory)
        {
            FactoryTable table = new FactoryTable();
            return table.Delete(factory);  
        }
        #endregion

        #region base station information object define
        public static BaseStat GetBaseStatByID(int id)
        {
            BaseStatTable table = new BaseStatTable();
            return table.GetBaseStatByID(id);
        }

        public static List<BaseStat> GetBaseStatAll()
        {
            BaseStatTable table = new BaseStatTable();
            return table.GetBaseStatAll();  
        }

        public static bool UpdateBaseStat(BaseStat baseStat)
        {
            BaseStatTable table = new BaseStatTable();
            return table.Update(baseStat);  
        }

        public static bool InsertBaseStat(BaseStat baseStat)
        {
            BaseStatTable table = new BaseStatTable();
            return table.Insert(baseStat);     
        }

        public static bool DeleteBaseStat(BaseStat baseStat)
        {
            BaseStatTable table = new BaseStatTable();
            return table.Delete(baseStat);  
        }
        #endregion
    }
}

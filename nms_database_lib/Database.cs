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

        public static Site GetSiteById(int id)
        {
            SiteTable table = new SiteTable();
            return table.GetSiteById(id);
        }

        public static List<Site> GetSiteListByPid(int pid)
        {
            SiteTable table = new SiteTable();
            return table.GetSiteListByPid(pid);
        }

        public static List<Site> GetSiteAll()
        {
            SiteTable table = new SiteTable();
            return table.GetSiteAll();
        }

        public static bool SiteUpdate(Site site)
        {
            SiteTable table = new SiteTable();
            return table.Update(site);
        }

        public static bool SiteInsert(Site site)
        {
            SiteTable table = new SiteTable();
            return table.Insert(site);
        }

        public static bool SiteDelete(Site site)
        {
            SiteTable table = new SiteTable();
            return table.Delete(site);
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

        #region area object table operation function
        public static Area GetAreaByID(int id)
        {
            AreaTable table = new AreaTable();
            return table.GetRegionByID(id);
        }

        public static Area GetAreaByName(string name)
        {
            AreaTable table = new AreaTable();
            return table.GetRegionByName(name);
        }

        public static List<Area> GetAreaAll()
        {
            AreaTable table = new AreaTable();
            return table.GetRegionAll();
        }

        public static List<Area> GetAreaAllByPid(int pid)
        {
            AreaTable table = new AreaTable();
            return table.GetAreaAllByPid(pid);
        }

        public static bool UpdateArea(Area area)
        {
            AreaTable table = new AreaTable();
            return table.Update(area);
        }

        public static bool InsertArea(Area area)
        {
            AreaTable table = new AreaTable();
            return table.Insert(area);
        }

        public static bool DeleteArea(Area area)
        {
            AreaTable table = new AreaTable();
            return table.Delete(area);
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

        #region moid object table
        public static SiteMoid GetSiteMoidByMoid(string tableName, ushort moid)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.GetSiteMoidByMoid(tableName, moid);
        }

        public static List<SiteMoid> GetSiteMoidAll(string tableName)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.GetSiteMoidAll(tableName);
        }

        public static bool DeleteSiteMoidTableItem(string tableName, SiteMoid moid)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.DeleteSiteMoidTableItem(tableName, moid);
        }

        public static bool DeleteSiteMoidTableItemAll(string tableName)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.DeleteSiteMoidTableItemAll(tableName);
        }

        public static bool DeleteSiteMoidTable(string tableName)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.DeleteSiteMoidTable(tableName);
        }

        public static bool CreateSiteMoidTable(string tableName)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.CreateSiteMoidTable(tableName);
        }

        public static bool InsertSiteMoidItem(string tableName, SiteMoid moid)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.InsertSiteMoidItem(tableName, moid);
        }

        public static bool InsertSiteMoidTableItem(string tableName, List<SiteMoid> moidList)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.InsertSiteMoidTableItem(tableName, moidList);
        }

        public static bool UpdateSiteMoidTableItem(string tableName, SiteMoid moid)
        {
            SiteMoidTable table = new SiteMoidTable();
            return table.UpdateSiteMoidTableItem(tableName, moid);
        }
        #endregion

        #region tree view table define  
        public static Tree GetTreeById(int id)
        {
            TreeTable table = new TreeTable();
            return table.GetTreeById(id);
        }

        public static List<Tree> GetTreeAll()
        {
            TreeTable table = new TreeTable();
            return table.GetTreeAll();
        }

        public static bool DeleteTreeById(Tree tree)
        {
            TreeTable table = new TreeTable();
            return table.DeleteTreeById(tree);
        }

        public static bool DeleteTreeAll()
        {
            TreeTable table = new TreeTable();
            return table.DeleteTreeAll();
        }

        public static bool InsertTreeElement(Tree tree)
        {
            TreeTable table = new TreeTable();
            return table.InsertTreeElement(tree);
        }

        public static bool UpdateTreeElement(Tree tree)
        {
            TreeTable table = new TreeTable();
            return table.UpdateTreeElement(tree);
        }
        #endregion
    }
}

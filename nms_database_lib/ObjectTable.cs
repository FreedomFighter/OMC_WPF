using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_database_lib
{
    #region moid operation
    class MoidTable
    {       
        private Moid ParserObjectFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = (int)row[MoidColumn.ID.ToString()];

                ushort moid = 0;
                strLine = row[MoidColumn.Moid.ToString()].ToString();
                if (string.IsNullOrEmpty(strLine) == false)
                {
                    moid = Convert.ToUInt16(strLine, 16);
                }

                string enname = row[MoidColumn.Enname.ToString()].ToString();
                string cnname = row[MoidColumn.Cnname.ToString()].ToString();
                bool only = row[MoidColumn.Readonly.ToString()].ToString() == "true" ? true : false;
                string type = row[MoidColumn.Type.ToString()].ToString();
                
                int level = 0;
                strLine = row[MoidColumn.AlarmLevel.ToString()].ToString();
                if (string.IsNullOrEmpty(strLine) == false)
                {
                    level = Convert.ToInt32(strLine);
                }

                int length = 0;
                strLine = row[MoidColumn.Length.ToString()].ToString();
                if (string.IsNullOrEmpty(strLine) == false)
                {
                    length = Convert.ToInt32(strLine);
                }

                string value = row[MoidColumn.Value.ToString()].ToString();
                string unit = row[MoidColumn.Unit.ToString()].ToString();
                string system = row[MoidColumn.SystemType.ToString()].ToString();

                return new Moid(id, level, length, only, moid, unit, type, enname, cnname, value, system);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public Moid GetMoidByMoid(ushort moid)
        {
            Moid obj = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_cmcc where Moid = {0}", moid);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    obj = ParserObjectFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return obj;
        }

        public List<Moid> GetMoidAll()
        {
            List<Moid> objList = new List<Moid>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_cmcc order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    Moid moid = ParserObjectFromDataRow(row);
                    if (moid != null)
                    {
                        objList.Add(moid);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return objList;
        }
    }
    #endregion

    #region site operation table function
    class SiteTable
    {
        private Site ParserObjectFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = 0;
                strLine = row[SiteColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                int pid = 0;
                strLine = row[SiteColumn.PID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    pid = Convert.ToInt32(strLine);
                }

                uint siteid = 0;
                strLine = row[SiteColumn.SiteId.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    siteid = Convert.ToUInt32(strLine);
                }

                byte subid = 0;
                strLine = row[SiteColumn.SubId.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    subid = Convert.ToByte(strLine);
                }

                byte type = 0;
                strLine = row[SiteColumn.DeviceType.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    type = Convert.ToByte(strLine);
                }

                byte mode = 0;
                strLine = row[SiteColumn.Model.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    mode = Convert.ToByte(strLine);
                }

                uint address = 0;
                strLine = row[SiteColumn.IPAddress.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    address = Convert.ToUInt32(strLine);
                }

                ushort port = 0;
                strLine = row[SiteColumn.Port.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    port = Convert.ToUInt16(strLine);
                }
                
                string telphone = row[SiteColumn.Port.ToString()].ToString();
                string moids = row[SiteColumn.Moidlist.ToString()].ToString();

                Site site = new Site(id, pid, siteid, subid, type, mode, address, port, telphone, moids);
                site.SiteName = row[SiteColumn.SiteName.ToString()].ToString();
                site.sProtocol = row[SiteColumn.sProtocol.ToString()].ToString();
                site.sAddress = row[SiteColumn.sAddress.ToString()].ToString();
                site.sFactory = row[SiteColumn.sFactory.ToString()].ToString();
                site.SiteType = (byte)row[SiteColumn.SiteType.ToString()];
                site.CommMode = (byte)row[SiteColumn.CommMode.ToString()];

                return site;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public Site GetSiteBySiteId(uint siteid, byte subid)
        {
            Site site = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_site where {0} = {1} and {2} = {3}", SiteColumn.SiteId.ToString(), siteid, SiteColumn.SubId.ToString(), subid);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    site = ParserObjectFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return site;
        }

        public List<Site> GetSiteAll()
        {
            List<Site> siteList = new List<Site>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_site order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    Site site = ParserObjectFromDataRow(row);
                    if (site != null)
                    {
                        siteList.Add(site);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return siteList;
        }

        public bool Update(Site site)
        {
            try
            {
                string query = string.Format("update tbl_RptInfo set {0]={1},{2}={3],{4}={5},{6}={7},{8}={9},{10}='{11}',{12}='{13}' where {14} = {15} and {16} = {17}", SiteColumn.PID.ToString(), site.PID, SiteColumn.DeviceType.ToString(), site.DeviceType, SiteColumn.Model.ToString(), site.Model, SiteColumn.IPAddress.ToString(), site.IPAddress, SiteColumn.Port.ToString(), site.Port, SiteColumn.Telphone.ToString(), site.Telphone, SiteColumn.Moidlist.ToString(), site.Moidlist, SiteColumn.SiteId.ToString(), site.SiteId, SiteColumn.SubId.ToString(), site.SubId);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(Site site)
        {
            try
            {
                string query = string.Format("insert into tbl_site ({0],{1},{2},{3},{4},{5},{6},{7},{8}) values ({9},{10},{11},{12},{13},{14},{15},'{16}','{17}')", SiteColumn.PID.ToString(), SiteColumn.SiteId.ToString(), SiteColumn.SubId.ToString(), SiteColumn.DeviceType.ToString(), SiteColumn.Model.ToString(), SiteColumn.IPAddress.ToString(), SiteColumn.Port.ToString(), SiteColumn.Telphone.ToString(), SiteColumn.Moidlist.ToString(), site.PID, site.SiteId, site.SubId, site.DeviceType, site.Model, site.IPAddress, site.Port,site.Telphone, site.Moidlist);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region user table object define
    class UserTable
    {
        private User ParserUserFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = 0;
                strLine = row[UserColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                int groupID = 0;
                strLine = row[UserColumn.GroupID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    groupID = Convert.ToInt32(strLine);
                }
                
                string username = row[UserColumn.UserName.ToString()].ToString();
                string password = row[UserColumn.Password.ToString()].ToString();
                string sex = row[UserColumn.Sex.ToString()].ToString();
                string email = row[UserColumn.Email.ToString()].ToString();
                string telphone = row[UserColumn.Telphone.ToString()].ToString();
                string createdate = row[UserColumn.CreateDate.ToString()].ToString();

                return new User(id, groupID, username, password, sex, email, telphone, createdate);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public User GetUserByName(string username)
        {
            User user = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_user where {0} = '{1}'", UserColumn.UserName.ToString(), username);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    user = ParserUserFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return user;
        }

        public List<User> GetUserAll()
        {
            List<User> userList = new List<User>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_user order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                userList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    User user = ParserUserFromDataRow(row);
                    if (null != user)
                    {
                        userList.Add(user);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return userList;
        }

        public bool Update(User user)
        {
            try
            {
                string query = string.Format("update tbl_user set UserName='{0}',Password='{1]',GroupID={2},Sex='{3}',Email='{4}',Telphone='{5}' where ID = {6}", user.UserName, user.Password, user.GroupID, user.Sex, user.Email, user.Telphone, user.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(User user)
        {
            try
            {
                string query = string.Format("insert into tbl_user (UserNam,Password,GroupID,Sex,Email,Telphone,CreateDate) values ('{0}','{1}',{2},'{3}','{4}','{5}','{6}')", user.UserName, user.Password, user.GroupID, user.Sex, user.Email, user.Telphone, user.CreateDate);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Delete(User user)
        {
            try
            {
                string query = string.Format("delete from tbl_user where ID = {0}", user.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region timeout table object define
    class TimeoutTable
    {
        private Timeout ParserTimeoutFromDataRow(DataRow row)
        {
            try
            {
                int timeout = 0;
                string strLine = row[TimeoutColumn.Timeout.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    timeout = Convert.ToInt32(strLine);
                }
               
                return new Timeout(timeout);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public List<Timeout> GetTimeoutAll()
        {
            List<Timeout> timeoutList = new List<Timeout>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_timeout order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                timeoutList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    Timeout timeout = ParserTimeoutFromDataRow(row);
                    if (null != timeout)
                    {
                        timeoutList.Add(timeout);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return timeoutList;
        }
    }
    #endregion

    #region group table object define 
    class GroupTable
    {
        private Group ParserGroupFromDataRow(DataRow row)
        {
            try
            {
                int id = 0;
                string strLine = row[GroupColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                string groupname = row[GroupColumn.GroupName.ToString()].ToString();
                string createdate = row[GroupColumn.CreateDate.ToString()].ToString();

                return new Group(id, groupname, createdate);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public Group GetGroupByID(int id)
        {
            Group group = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_group where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    group = ParserGroupFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return group;
        }

        public Group GetGroupByName(string groupname)
        {
            Group group = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_group where GroupName = '{0}'", groupname);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    group = ParserGroupFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return group;
        }

        public List<Group> GetGroupAll()
        {
            List<Group> groupList = new List<Group>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_group order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                groupList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    Group group = ParserGroupFromDataRow(row);
                    if (null != group)
                    {
                        groupList.Add(group);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return groupList;
        }

        public bool Update(Group group)
        {
            try
            {
                string query = string.Format("update tbl_group set GroupName='{0}',CreateDate='{1]' where ID = {2}", group.GroupName, group.CreateDate, group.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(Group group)
        {
            try
            {
                string query = string.Format("insert into tbl_group (GroupName,CreateDate) values ('{0}','{1}')", group.GroupName, group.CreateDate);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Delete(Group group)
        {
            try
            {
                string query = string.Format("delete from tbl_group where ID = {0}", group.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region enumex table object define
    class EnumExTable
    {
        private EnumEx ParserEnumExFromDataRow(DataRow row)
        {
            try
            {
                int id = 0;
                string strLine = row[EnumExColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                string name = row[EnumExColumn.sName.ToString()].ToString();
                string key = row[EnumExColumn.sKey.ToString()].ToString();
                string value = row[EnumExColumn.sValue.ToString()].ToString();

                return new EnumEx(id, name, key, value);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public List<EnumEx> GetEnumExAll()
        {
            List<EnumEx> enumExList = new List<EnumEx>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_enum order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                enumExList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    EnumEx enumex = ParserEnumExFromDataRow(row);
                    if (null != enumex)
                    {
                        enumExList.Add(enumex);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return enumExList;
        }
    }
    #endregion

    #region region table object define
    class RegionTable
    {
        private Region ParserRegionFromDataRow(DataRow row)
        {
            try
            {
                int id = 0;
                string strLine = row[RegionColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                int regionid = 0;
                strLine = row[RegionColumn.RegionID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    regionid = Convert.ToInt32(strLine);
                }

                string regionname = row[RegionColumn.RegionName.ToString()].ToString();
                string createdate = row[RegionColumn.CreateDate.ToString()].ToString();

                return new Region(id, regionid, regionname, createdate);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public Region GetRegionByID(int id)
        {
            Region region = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_region where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    region = ParserRegionFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return region;
        }

        public Region GetRegionByName(string name)
        {
            Region region = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_region where RegionName = '{0}'", name);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    region = ParserRegionFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return region;
        }

        public List<Region> GetRegionAll()
        {
            List<Region> regionList = new List<Region>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_region order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                regionList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    Region region = ParserRegionFromDataRow(row);
                    if (null != region)
                    {
                        regionList.Add(region);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return regionList;
        }

        public bool Update(Region region)
        {
            try
            {
                string query = string.Format("update tbl_region set RegionID={0},RegionName='{1]',CreateDate='{2}' where ID = {2}", region.RegionID, region.RegionName, region.CreateDate,region.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(Region region)
        {
            try
            {
                string query = string.Format("insert into tbl_region (RegionID,RegionName,CreateDate) values ({0},'{1}','{2}')", region.RegionID,region.RegionName, region.CreateDate);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Delete(Region region)
        {
            try
            {
                string query = string.Format("delete from tbl_region where ID = {0}", region.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region alarm table object define
    class AlarmTable
    {
        private Alarm ParserAlarmFromDataRow(DataRow row)
        {
            try
            {
                int id = 0;
                string strLine = row[AlarmColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                string subid = row[AlarmColumn.SubID.ToString()].ToString();
                string siteid = row[AlarmColumn.SiteID.ToString()].ToString();
                string alarmname = row[AlarmColumn.AlarmName.ToString()].ToString();
                string alarmtime = row[AlarmColumn.AlarmTime.ToString()].ToString();
                string alarmlevel = row[AlarmColumn.AlarmLevel.ToString()].ToString();
                string sitename= row[AlarmColumn.SiteName.ToString()].ToString();

                return new Alarm(id, siteid, subid, alarmname, alarmtime, alarmlevel, sitename);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public Alarm GetAlarmByID(int id)
        {
            Alarm alarm = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_alarm where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    alarm = ParserAlarmFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return alarm;
        }              

        public List<Alarm> GetAlarmAll()
        {
            List<Alarm> alarmList = new List<Alarm>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_alarm order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                alarmList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    Alarm alarm = ParserAlarmFromDataRow(row);
                    if (null != alarm)
                    {
                        alarmList.Add(alarm);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return alarmList;
        }

        public bool Update(Alarm alarm)
        {
            try
            {
                string query = string.Format("update tbl_alarm set SiteID='{0}',SubID='{1]',AlarmName='{2}',AlarmTime='{3}',AlarmLevel='{4}',SiteName='{5}' where ID = {6}", alarm.SiteId, alarm.SubId, alarm.AlarmName, alarm.AlarmTime, alarm.AlarmLevel, alarm.SiteName, alarm.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(Alarm alarm)
        {
            try
            {
                string query = string.Format("insert into tbl_alarm (SiteID,SubID,AlarmName,AlarmTime,AlarmLevel,SiteName) values ({0},'{1}','{2}','{3}','{4}','{5}')", alarm.SiteId, alarm.SubId, alarm.AlarmName, alarm.AlarmTime, alarm.AlarmLevel, alarm.SiteName);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Delete(Alarm alarm)
        {
            try
            {
                string query = string.Format("delete from tbl_alarm where ID = {0}", alarm.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region alarm log object define
    public class AlarmLogTable
    {
        private AlarmLog ParserAlarmLogFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = 0;
                strLine = row[AlarmLogColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                byte subid = 0;
                strLine = row[AlarmLogColumn.SubID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToByte(strLine);
                }

                uint siteid = 0;
                strLine = row[AlarmLogColumn.SiteID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    siteid = Convert.ToUInt32(strLine);
                }

                int index = 0;
                strLine = row[AlarmLogColumn.AlarmIndex.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    index = Convert.ToInt32(strLine);
                }

                int city = 0;
                strLine = row[AlarmLogColumn.City.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    city = Convert.ToInt32(strLine);
                }

                int province = 0;
                strLine = row[AlarmLogColumn.Province.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    province = Convert.ToInt32(strLine);
                }

                int level = 0;
                strLine = row[AlarmLogColumn.AlarmLevel.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    level = Convert.ToInt32(strLine);
                }

                string time = row[AlarmLogColumn.AlarmTime.ToString()].ToString();
                string detail = row[AlarmLogColumn.Detail.ToString()].ToString();
                string aitem = row[AlarmLogColumn.AltemNum.ToString()].ToString();

                return new AlarmLog(id, siteid, subid, time, index, detail, province, city, aitem, level);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public AlarmLog GetAlarmLogByID(int id)
        {
            AlarmLog alarmLog = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_alrmlog where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    alarmLog = ParserAlarmLogFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return alarmLog;
        }

        public List<AlarmLog> GetAlarmLogAll()
        {
            List<AlarmLog> alarmLogList = new List<AlarmLog>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_alrmlog order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                alarmLogList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    AlarmLog alarmLog = ParserAlarmLogFromDataRow(row);
                    if (null != alarmLog)
                    {
                        alarmLogList.Add(alarmLog);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return alarmLogList;
        }

        public bool InsertAlarmLog(AlarmLog alarmLog)
        {
            try
            {
                string query = string.Format("insert into tbl_alrmlog (SiteID, SubID, AlarmTime, AlarmIndex, Detail, Province, City, AltemNum, AlarmLevel) values ({0},{1},'{2}',{3},'{4}',{5},{6},'{7}',{8})", alarmLog.SiteID, alarmLog.SubID, alarmLog.AlarmTime, alarmLog.AlarmIndex, alarmLog.Detail, alarmLog.Province, alarmLog.City, alarmLog.AItemNum, alarmLog.AlarmLevel);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool DeleteAlarmLog(AlarmLog alarmLog)
        {
            try
            {
                string query = string.Format("delete from tbl_alrmlog where ID = {0}", alarmLog.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region operation log object define
    public class OpsLogTable
    {
        private OpsLog ParserOpsLogFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = 0;
                strLine = row[OpsLogColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }
                       
                int userid = 0;
                strLine = row[OpsLogColumn.OprUserID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    userid = Convert.ToInt32(strLine);
                }

                int city = 0;
                strLine = row[OpsLogColumn.City.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    city = Convert.ToInt32(strLine);
                }

                int province = 0;
                strLine = row[OpsLogColumn.Province.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    province = Convert.ToInt32(strLine);
                }

                string time = row[OpsLogColumn.OprTime.ToString()].ToString();
                string detail = row[OpsLogColumn.Detail.ToString()].ToString();

                return new OpsLog(id, userid, city, province, time, detail);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public OpsLog GetOpsLogByID(int id)
        {
            OpsLog alarmLog = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_opslog where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    alarmLog = ParserOpsLogFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return alarmLog;
        }

        public List<OpsLog> GetOpsLogAll()
        {
            List<OpsLog> oprLogList = new List<OpsLog>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_opslog order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                oprLogList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    OpsLog oprLog = ParserOpsLogFromDataRow(row);
                    if (null != oprLog)
                    {
                        oprLogList.Add(oprLog);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return oprLogList;
        }

        public bool InsertOpsLog(OpsLog oprLog)
        {
            try
            {
                string query = string.Format("insert into tbl_opslog (OprUserID, OprTime, Detail, Province, City) values ({0},'{1}','{2}',{3},{4},{5})", oprLog.OprUserID, oprLog.OprTime, oprLog.Detail, oprLog.Province, oprLog.City);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool DeleteOpsLog(OpsLog oprLog)
        {
            try
            {
                string query = string.Format("delete from tbl_opslog where ID = {0}", oprLog.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region factory information object define
    public class FactoryTable
    {
        private Factory ParserFactoryFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = 0;
                strLine = row[FactoryColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                int factid = 0;
                strLine = row[FactoryColumn.FactID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    factid = Convert.ToByte(strLine);
                }

                string factName = row[FactoryColumn.FactName.ToString()].ToString();
                string factFlag = row[FactoryColumn.FactFlag.ToString()].ToString();
                string factAddr = row[FactoryColumn.FactAddr.ToString()].ToString();
                string linkMan = row[FactoryColumn.LinkMan.ToString()].ToString();
                string linkTel = row[FactoryColumn.LinkTel.ToString()].ToString();
                string email = row[FactoryColumn.Email.ToString()].ToString();
                string note = row[FactoryColumn.Note.ToString()].ToString();

                return new Factory(id, factid, factName, factFlag, factAddr, linkMan, linkTel, email, note);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public Factory GetFactoryByID(int id)
        {
            Factory factory = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_factory where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    factory = ParserFactoryFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return factory;
        }

        public List<Factory> GetFactoryAll()
        {
            List<Factory> factoryList = new List<Factory>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_factory order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                factoryList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    Factory factory = ParserFactoryFromDataRow(row);
                    if (null != factory)
                    {
                        factoryList.Add(factory);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return factoryList;
        }

        public bool Update(Factory factory)
        {
            try
            {
                string query = string.Format("update tbl_factory set FactID={0},FactName='{1]',FactFlag='{2}',FactAddr='{3}',LinkMan='{4}',LinkTel='{5}',Email='{6}',Note='{7}' where ID = {8}", factory.FactID, factory.FactName, factory.FactFlag, factory.FactAddr, factory.LinkMan, factory.LinkTel, factory.Email, factory.Note, factory.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(Factory factory)
        {
            try
            {
                string query = string.Format("insert into tbl_factory (FactID, FactName, FactFlag, FactAddr, LinkMan, LinkTel, Email, Note) values ({0},'{1}','{2}','{3}','{4}','{5'},'{6}','{7}')", factory.FactID, factory.FactName, factory.FactFlag, factory.FactAddr, factory.LinkMan, factory.LinkTel, factory.Email, factory.Note);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Delete(Factory factory)
        {
            try
            {
                string query = string.Format("delete from tbl_factory where ID = {0}", factory.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region base station information object define
    public class BaseStatTable
    {
        private BaseStat ParserBaseStatFromDataRow(DataRow row)
        {
            try
            {
                string strLine = string.Empty;

                int id = 0;
                strLine = row[BaseStatColumn.ID.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    id = Convert.ToInt32(strLine);
                }

                int City = 0;
                strLine = row[BaseStatColumn.City.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    City = Convert.ToByte(strLine);
                }

                int Province = 0;
                strLine = row[BaseStatColumn.Province.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    Province = Convert.ToInt32(strLine);
                }

                int Code = 0;
                strLine = row[BaseStatColumn.BaseStatCode.ToString()].ToString();
                if (false == string.IsNullOrEmpty(strLine))
                {
                    Code = Convert.ToInt32(strLine);
                }

                string BaseStatName = row[BaseStatColumn.BaseStatName.ToString()].ToString();
                string CID = row[BaseStatColumn.CID.ToString()].ToString();
                string X = row[BaseStatColumn.X.ToString()].ToString();
                string Y = row[BaseStatColumn.Y.ToString()].ToString();
                string PNBCCH64 = row[BaseStatColumn.PNBCCH64.ToString()].ToString();
                string Detail = row[BaseStatColumn.Detail.ToString()].ToString();
              
                return new BaseStat(id, BaseStatName, CID, X, Y, PNBCCH64, Province, City, Code, Detail);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return null;
        }

        public BaseStat GetBaseStatByID(int id)
        {
            BaseStat baseStat = null;

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_basestat where ID = {0}", id);
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    baseStat = ParserBaseStatFromDataRow(row);
                    break;
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return baseStat;
        }

        public List<BaseStat> GetBaseStatAll()
        {
            List<BaseStat> baseStatList = new List<BaseStat>();

            try
            {
                DataTable table = new DataTable();
                string query = string.Format("select * from tbl_basestat order by ID");
                OdbcDataAdapter adapter = new OdbcDataAdapter(query, RuntimeObjects.Connection);
                adapter.Fill(table);

                baseStatList.Clear();
                foreach (DataRow row in table.Rows)
                {
                    BaseStat baseStat = ParserBaseStatFromDataRow(row);
                    if (null != baseStat)
                    {
                        baseStatList.Add(baseStat);
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }

            return baseStatList;
        }

        public bool Update(BaseStat baseStat)
        {
            try
            {
                string query = string.Format("update tbl_basestat set BaseStatName='{0}', CID='{1}', X='{2}', Y='{3}', PNBCCH64='{4}', Province={5}, City={6}, BaseStatCode={7}, Detail='{8}' where ID = {9}", baseStat.BaseStatName, baseStat.CID, baseStat.X, baseStat.Y, baseStat.PNBCCH64, baseStat.Province, baseStat.City, baseStat.BaseStatCode, baseStat.Detail, baseStat.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Insert(BaseStat baseStat)
        {
            try
            {
                string query = string.Format("insert into tbl_basestat (BaseStatName, CID, X, Y, PNBCCH64, Province, City, BaseStatCode, Detail) values ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},'{8}')", baseStat.BaseStatName, baseStat.CID, baseStat.X, baseStat.Y, baseStat.PNBCCH64, baseStat.Province, baseStat.City, baseStat.BaseStatCode, baseStat.Detail);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }

        public bool Delete(BaseStat baseStat)
        {
            try
            {
                string query = string.Format("delete from tbl_basestat where ID = {0}", baseStat.ID);
                OdbcCommand command = new OdbcCommand(query, RuntimeObjects.Connection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return false;
            }
        }
    }
    #endregion

    #region 全局数据库链接定义类
    // 运行时所用到的全局对象
    public class RuntimeObjects
    {
        public static readonly OdbcConnection Connection; // Runtime.mdb连接句柄

        static RuntimeObjects()
        {
            string ProgramPath = System.IO.Directory.GetCurrentDirectory(); // 应用程序执行路径
            string dbRuntimePath = string.Format("{0}{1}", ProgramPath, @"\nms_data.mdb");

            try
            {
                // 判断文件是否存在
                if (File.Exists(dbRuntimePath))
                {
                    Connection = new OdbcConnection(GetAccessConnection(dbRuntimePath, false));
                    Connection.Open();
                }
                else
                {
                    throw new Exception("File not found: " + dbRuntimePath);
                }
            }
            catch (Exception r)
            {
                if (Connection != null)
                {
                    Connection.Close();
                    Connection = null;
                }
                Console.WriteLine(r.Message);
            }
        }

        /// <summary>
        /// 获取Access数据库的连接字符串(ODBC)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static string GetAccessConnection(string pathname, bool readOnly)
        {
            string driver = "Driver={Microsoft Access Driver (*.mdb)}";
            string dbpath = ";DBQ=" + pathname;
            string rwflag = ";Readonly=" + (readOnly ? "1" : "0");
            string pwd = ";pwd=netopsystem"; //password
            return (driver + dbpath + rwflag + pwd);
        }
    }
    #endregion
}

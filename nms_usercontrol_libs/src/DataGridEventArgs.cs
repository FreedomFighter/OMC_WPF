using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_usercontrol_libs.src
{
    public delegate void DataGridOperationHandler(object sender, DataGridEventArgs e);

    public enum DataGridOperMode
    {
        None = 0x00, Setting, Query, Unknow,
    }

    public class DataGridEventArgs
    {
        private DataGridOperMode mode = DataGridOperMode.None;
        public DataGridOperMode Mode
        {
            get { return mode; }
        }

        private List<SiteModel> siteList = null;
        public List<SiteModel> SiteList
        {
            get { return siteList; }
        }

        public DataGridEventArgs(List<SiteModel> list, DataGridOperMode _mode)
        {
            if (null == list)
            {
                return;
            }

            mode = _mode;
            siteList = new List<SiteModel>();
            foreach (SiteModel element in list)
            {
                siteList.Add(element);
            }
        }
    }
}

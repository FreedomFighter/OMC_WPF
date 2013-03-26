using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_usercontrol_libs.src
{
    public class PropertyNodeItem
    {
        public const string RPTICON0 = "/nms_usercontrol_libs;component/images/wireless_0_16.png";
        public const string RPTICON1 = "/nms_usercontrol_libs;component/images/wireless_1_16.png";
        public const string RPTICON2 = "/nms_usercontrol_libs;component/images/wireless_2_16.png";
        public const string RPTICON3 = "/nms_usercontrol_libs;component/images/wireless_3_16.png";
        public const string RPTICON4 = "/nms_usercontrol_libs;component/images/wireless_4_16.png";
        public const string RPTICON5 = "/nms_usercontrol_libs;component/images/wireless_5_16.png";

        public const string CITYICON = "/nms_usercontrol_libs;component/images/city_16.png";
        public const string EDITICON = "/nms_usercontrol_libs;component/images/edit_16.png";


        public int Id { get; set; }
        public int NodeType { get; set; }
        public int NodeId { get; set; }
        public string DisplayName { get; set; }
        public string ToolTips { get; set; }
        public string Icon { get; set; }
        public string EditIcon { get; set; }


        public List<PropertyNodeItem> Children { get; set; }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
    }
}

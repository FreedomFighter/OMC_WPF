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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace nms_usercontrol_libs.src
{
    /// <summary>
    /// SiteTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class SiteTreeView : UserControl
    {
        private List<SiteModel> SiteLists = new List<SiteModel>();
        private const string RPTICON = "/nms_usercontrol_libs;component/images/computer_24.png";

        public SiteTreeView()
        {
            InitializeComponent();
            ShowTreeView();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ShowTreeView();
        }


        private void ShowTreeView()
        {
            List<PropertyNodeItem> itemList = new List<PropertyNodeItem>();
            PropertyNodeItem node1 = new PropertyNodeItem()
            {
                DisplayName = "Node No.1",
                Name = "This is the discription of Node1. This is a folder.",
                Icon = RPTICON,
            };

            PropertyNodeItem node1tag1 = new PropertyNodeItem()
            {
                DisplayName = "Tag No.1",
                Name = "This is the discription of Tag 1. This is a tag.",
                Icon = RPTICON,
                EditIcon = "/nms_usercontrol_libs;component/images/refresh_24.png"
            };
            node1.Children.Add(node1tag1);

            PropertyNodeItem node1tag2 = new PropertyNodeItem()
            {
                DisplayName = "Tag No.2",
                Name = "This is the discription of Tag 2. This is a tag.",
                Icon = "/nms_usercontrol_libs;component/images/computer_24.png",
                EditIcon = "/nms_usercontrol_libs;component/images/refresh_24.png"
            };
            node1.Children.Add(node1tag2);
            itemList.Add(node1);

            PropertyNodeItem node2 = new PropertyNodeItem()
            {
                DisplayName = "Node No.2",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = RPTICON,
            };

            PropertyNodeItem node2tag3 = new PropertyNodeItem()
            {
                DisplayName = "Tag No.3",
                Name = "This is the discription of Tag 3. This is a tag.",
                Icon = RPTICON,
                EditIcon = "/nms_usercontrol_libs;component/images/refresh_24.png"
            };
            node2.Children.Add(node2tag3);

            PropertyNodeItem node2tag4 = new PropertyNodeItem()
            {
                DisplayName = "Tag No.4",
                Name = "This is the discription of Tag 4. This is a tag.",
                Icon = RPTICON,
                EditIcon = "/nms_usercontrol_libs;component/images/refresh_24.png"
            };
            node2.Children.Add(node2tag4);
            itemList.Add(node2);

            this.SiteListTreeView.ItemsSource = itemList;
        }

        private void BtnRepeaterEdit_Click(object sender, MouseButtonEventArgs e)
        {

            MessageBox.Show("DDDDDDDDDDDD");
        }


        /*
        public void BindTreeView()
        {
            PropertyNodeItem node1 = new PropertyNodeItem("a1", "中国", 0);
            PropertyNodeItem node2 = new PropertyNodeItem("a2", "北京市", 1);
            PropertyNodeItem node3 = new PropertyNodeItem("a3", "吉林省", 1);
            PropertyNodeItem node4 = new PropertyNodeItem("a4", "上海市", 1);
            node1.Items.Add(node2);
            node1.Items.Add(node3);
            node1.Items.Add(node4);
            PropertyNodeItem node5 = new PropertyNodeItem("5", "海淀区", 2);
            PropertyNodeItem node6 = new PropertyNodeItem("6", "朝阳区", 2);
            PropertyNodeItem node7 = new PropertyNodeItem("7", "大兴区", 2);
            node2.Items.Add(node5);
            node2.Items.Add(node6);
            node2.Items.Add(node7);
            PropertyNodeItem node8 = new PropertyNodeItem("8", "白山市", 2);
            PropertyNodeItem node9 = new PropertyNodeItem("9", "长春市", 2);
            node3.Items.Add(node8);
            node3.Items.Add(node9);
            PropertyNodeItem node10 = new PropertyNodeItem("10", "抚松县", 3);
            PropertyNodeItem node11 = new PropertyNodeItem("11", "靖宇县", 3);
            node8.Items.Add(node10);
            node8.Items.Add(node11);
            this.SiteListTreeView.Items.Clear();
            this.SiteListTreeView.Items.Add(node1);
        }*/

    }

    /*
     public class PropertyNodeItem : TreeViewItem
     {
         public int Level { get; set; }
         public string Name { get; set; }

         public PropertyNodeItem(string name, string DisplayName, int level)
         {
             this.Level = level;
             this.Header = DisplayName;
             this.Name = name;
             // 在此点下面插入创建对象所需的代码。
         }
     }*/

    internal class PropertyNodeItem
    {
        public string Icon { get; set; }
        public string EditIcon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }

        public List<PropertyNodeItem> Children { get; set; }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
    }

}

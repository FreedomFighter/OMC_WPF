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
        private const string RPTICON = "/nms_usercontrol_libs;component/images/wireless_16.png";
        private const string CITYICON = "/nms_usercontrol_libs;component/images/city_16.png";
        private const string EDITICON = "/nms_usercontrol_libs;component/images/edit_16.png";
        //private const string ADDICON = "/nms_usercontrol_libs;component/images/add_16.png";


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
                DisplayName = "Guangzhou",
                Name = "This is the discription of Node1. This is a folder.",
                Icon = CITYICON,
            };

            PropertyNodeItem node1tag1 = new PropertyNodeItem()
            {
                DisplayName = "RPT00012210",
                Name = "This is the discription of Tag 1. This is a tag.",
                Icon = RPTICON,
                EditIcon = EDITICON,
            };
            node1.Children.Add(node1tag1);

            PropertyNodeItem node1tag2 = new PropertyNodeItem()
            {
                DisplayName = "RPT00011882",
                Name = "This is the discription of Tag 2. This is a tag.",
                Icon = RPTICON,
                EditIcon = EDITICON,
            };
            node1.Children.Add(node1tag2);
            itemList.Add(node1);

            PropertyNodeItem node2 = new PropertyNodeItem()
            {
                DisplayName = "Beijing",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = CITYICON,
            };

            PropertyNodeItem node2tag3 = new PropertyNodeItem()
            {
                DisplayName = "FO121093488",
                Name = "This is the discription of Tag 3. This is a tag.",
                Icon = RPTICON,
                EditIcon = EDITICON
            };
            node2.Children.Add(node2tag3);

            PropertyNodeItem node2tag4 = new PropertyNodeItem()
            {
                DisplayName = "FO121096097",
                Name = "This is the discription of Tag 4. This is a tag.",
                Icon = RPTICON,
                EditIcon = EDITICON
            };
            node2.Children.Add(node2tag4);
            itemList.Add(node2);


            PropertyNodeItem node3 = new PropertyNodeItem()
            {
                DisplayName = "Shanghai",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = CITYICON,
            };

            PropertyNodeItem node3sub1 = new PropertyNodeItem()
            {
                DisplayName = "Qingpu",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = CITYICON,
            };

            PropertyNodeItem node3sub1sub1 = new PropertyNodeItem()
            {
                DisplayName = "RPT01220123",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = RPTICON,
                EditIcon = EDITICON
            };
            node3sub1.Children.Add(node3sub1sub1);

            PropertyNodeItem node3sub1sub2 = new PropertyNodeItem()
            {
                DisplayName = "RPT8872137",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = RPTICON,
                EditIcon = EDITICON
            };
            node3sub1.Children.Add(node3sub1sub2);
            node3.Children.Add(node3sub1);

            PropertyNodeItem node3sub2 = new PropertyNodeItem()
            {
                DisplayName = "Minhang",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = CITYICON,
            };

            PropertyNodeItem node3sub2sub1 = new PropertyNodeItem()
            {
                DisplayName = "RPT001232112",
                Name = "This is the discription of Node 2. This is a folder.",
                Icon = RPTICON,
                EditIcon = EDITICON
            };
            node3sub2.Children.Add(node3sub2sub1);
            node3.Children.Add(node3sub2);

            itemList.Add(node3);

            this.SiteListTreeView.ItemsSource = itemList;
        }

        private void BtnRepeaterEdit_Click(object sender, MouseButtonEventArgs e)
        {

            MessageBox.Show("Edit button click event test");
        }

        private void BtnRepeaterAdd_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Add button click event test");
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("BtnAddNew_Click event test");
        }
    }

    internal class PropertyNodeItem
    {
        public string Icon { get; set; }
        public string EditIcon { get; set; }
        public string AddIcon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }

        public List<PropertyNodeItem> Children { get; set; }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();
        }
    }

}

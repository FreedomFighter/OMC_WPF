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
using nms_database_lib;

namespace nms_usercontrol_libs.src
{
    /// <summary>
    /// SiteTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class SiteTreeView : UserControl
    {
        public List<PropertyNodeItem> TreeListView = new List<PropertyNodeItem>();
        //private List<SiteModel> SiteLists = new List<SiteModel>();
        
        public SiteTreeView()
        {
            InitializeComponent();
            TreeViewInitial();
        }

        private void TreeViewInitial()
        {
            List<Tree> treeListData = Database.GetTreeAll();
            TreeDataDispConvert treeConver = new TreeDataDispConvert();
            treeConver.TreeViewInitial(treeListData, TreeListView);
            this.SiteListTreeView.ItemsSource = TreeListView;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ShowTreeView();
        }


        private void ShowTreeView()
        {
            PropertyNodeItem node1 = new PropertyNodeItem()
            {
                DisplayName = "Guangzhou",
                ToolTips = "This is the discription of Node1. This is a folder.",
                Icon = PropertyNodeItem.CITYICON,
            };

            PropertyNodeItem node1tag1 = new PropertyNodeItem()
            {
                DisplayName = "RPT00012210",
                ToolTips = "This is the discription of Tag 1. This is a tag.",
                Icon = PropertyNodeItem.RPTICON1,
            };
            node1.Children.Add(node1tag1);
            TreeListView.Add(node1);

            this.SiteListTreeView.ItemsSource = TreeListView;
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

}

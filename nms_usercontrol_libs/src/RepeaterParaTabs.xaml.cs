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
    /// RepeaterParaTabs.xaml 的交互逻辑
    /// </summary>
    public partial class RepeaterParaTabs : UserControl
    {
        //DataGridModel rptInfoDataGridModel = new DataGridModel();
        //DataGridModel monInfoDataGridModel = new DataGridModel();
        DataGridModel alrmInfoDataGridModel = new DataGridModel();
        DataGridModel AlarmEnDataGridModel = new DataGridModel();
        DataGridModel rfParamDataGridModel = new DataGridModel();
        DataGridModel rfStateDataGridModel = new DataGridModel();

        DataGridUserModel rptInfoDataGridModel = new DataGridUserModel();
        DataGridUserModel monInfoDataGridModel = new DataGridUserModel();
    
        // 操作回调事件函数
        public event DataGridOperationHandler DataGridOperationCompleted = null;

        public RepeaterParaTabs()
        {
            InitializeComponent();

            InitializeComponentBindingSource();
        }

        private void InitializeComponentBindingSource()
        {
            try
            {
                this.RepeaterInfoDataGrid.ItemsSource = rptInfoDataGridModel;
                this.MonitorInfoDataGrid.ItemsSource = monInfoDataGridModel;

                //this.RFParaDataGrid.ItemsSource = userList;
                //this.RFStatusDataGrid.ItemsSource = userList;
                //this.RFStatusDataGrid.ItemsSource = rfStateDataGridModel;
                //this.RepeaterInfoDataGrid.ItemsSource = rptInfoDataGridModel;
                //this.MonitorInfoDataGrid.ItemsSource = monInfoDataGridModel;
                this.AlarmInfoDataGrid.ItemsSource = alrmInfoDataGridModel;
                this.AlarmEnableDataGrid.ItemsSource = AlarmEnDataGridModel;
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private void AddRptInfoDataGridRow(SiteModel element)
        {
            try
            {
                if ((element.Moid & 0x0F00) == 0x0000)
                {
                    //rptInfoDataGridModel.Add(element);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private void AddMonitorInfoDataGridRow(SiteModel element)
        {
            try
            {
                if ((element.Moid & 0x0F00) == 0x0100)
                {
                    //monInfoDataGridModel.Add(element);

                    DataGridRow row = new DataGridRow();
                    
                    this.RepeaterInfoDataGrid.Items.Add(row);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private void AddRFInfoDataGridRow(SiteModel element)
        {
            try
            {
                if ((element.Moid & 0x0F00) == 0x0400)
                {
                    rfParamDataGridModel.Add(element);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private void AddRFStatusDataGridRow(SiteModel element)
        {
            try
            {
                if ((element.Moid & 0x0F00) == 0x0500)
                {
                    rfStateDataGridModel.Add(element);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private void AddAlarmInfoDataGridRow(SiteModel element)
        {
            try
            {
                if ((element.Moid & 0x0F00) == 0x0300)
                {
                    alrmInfoDataGridModel.Add(element);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        private void AddAlarmEnDataGridRow(SiteModel element)
        {
            try
            {
                if ((element.Moid & 0x0F00) == 0x0200)
                {
                    AlarmEnDataGridModel.Add(element);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        public void AddRepeaterMonitorParamForUI(List<SiteModel> siteModelList)
        {
            try
            {
                foreach (SiteModel element in siteModelList)
                {
                    switch (element.Moid & 0x0F00)
                    {
                        case 0x0000:
                            AddRptInfoDataGridRow(element);
                            break;
                        case 0x0100:
                            AddMonitorInfoDataGridRow(element);
                            break;
                        case 0x0200:
                            AddAlarmEnDataGridRow(element);
                            break;
                        case 0x0300:
                            AddAlarmInfoDataGridRow(element);
                            break;
                        case 0x0400:
                            AddRFInfoDataGridRow(element);
                            break;
                        case 0x0500:
                            AddRFStatusDataGridRow(element);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
        }

        public void RepeaterParamProcessForUI(object sender, DataGridEventArgs e)
        {
        }

        
        //public List<UserModel> userList = new List<UserModel>();
        private void BtnGet_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                //userList.Add(new UserModel("username" + i.ToString(), "password" + i.ToString(), "mail" + i.ToString()));
                rptInfoDataGridModel.CreateGenericSiteModelData(new UserModel("aaaaaaaaaaa", "1111111111", "aaaaaa@qqqqqqqq.com"));
                monInfoDataGridModel.CreateGenericSiteModelData(new UserModel("aaaaaaaaaaa", "1111111111", "aaaaaa@qqqqqqqq.com"));
            }
        }
    }
}

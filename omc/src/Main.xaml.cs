using nms_usercontrol_libs.src;
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
using System.Windows.Shapes;
using nms_comm_lib;
using nms_database_lib;
using Microsoft.Win32;
using nms_excel_lib;
using System.Data;

namespace omc.src
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        CommunicateBase communicateBase = null;

        public Main()
        {
            InitializeComponent();

            ToolBar.ButtonExitClickEvent += new UserLoginClickEventHandler(ButtonExitEventHandler);
        }

        /**************************************************
         * 退出应用程序处理函数
         * ***********************************************/
        private void ButtonExitEventHandler(object sender, UserEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TopMenuExitClick(object sender, RoutedEventArgs e)
        {
            
            if (MessageBox.Show("Are you sure to exit application?", "Exit Applicatoin", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
                        
        }
    }
}

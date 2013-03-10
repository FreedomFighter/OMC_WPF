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
    /// UserLogin.xaml 的交互逻辑
    /// </summary>
    /// 
    

    public partial class UserLogin : UserControl
    {
        public ButtonClickEventHandler ButtonLoginClickEvent = null;
        public ButtonClickEventHandler ButtonExitClickEvent = null;

        public UserLogin()
        {
            InitializeComponent();
        }

        /********************************************
         * 登录按钮事件处理函数
         * *****************************************/
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (null != ButtonLoginClickEvent)
            {
                ButtonLoginClickEvent(BtnLogin, new EventArgs());
            }
        }

        /********************************************
         * 退出按钮事件处理函数
         * *****************************************/
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (null != ButtonExitClickEvent)
            {
                ButtonExitClickEvent(BtnExit, new EventArgs());
            }
        }
    }
}

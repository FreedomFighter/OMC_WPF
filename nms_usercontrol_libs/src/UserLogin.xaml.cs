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
    public delegate void UserLoginClickEventHandler(object sender, UserEventArgs e);

    public partial class UserLogin : UserControl
    {
        public UserLoginClickEventHandler ButtonLoginClickEvent = null;
        public UserLoginClickEventHandler ButtonExitClickEvent = null;

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
                ButtonLoginClickEvent(BtnLogin, new UserEventArgs(UsernameTextBox.Text.Trim(), PasswordTextBox.Password));
            }
        }

        /********************************************
         * 退出按钮事件处理函数
         * *****************************************/
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (null != ButtonExitClickEvent)
            {
                ButtonExitClickEvent(BtnExit, new UserEventArgs(UsernameTextBox.Text.Trim(), PasswordTextBox.Password));
            }
        }
    }

    //===========================================================================================
    /// <summary>
    /// 定义登录窗口回调函数的参数事件类
    /// </summary>
    public class UserEventArgs : EventArgs
    {
        private string username;
        public string Username
        {
            get { return username; }
            private set { username = value; }
        }

        private string password;
        public string Password
        {
            get { return password; }
            private set { password = value; }
        }

        public UserEventArgs(string name, string passwd)
        {
            Username = name;
            Password = passwd;
        }
    }
}

using nms_usercontrol_libs.src;
using omc.src;
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
using nms_utility_lib;

namespace omc
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UserLoginView.ButtonExitClickEvent += new UserLoginClickEventHandler(ButtonExitEventHandler);
            UserLoginView.ButtonLoginClickEvent += new UserLoginClickEventHandler(ButtonLoginEventHandler);
        }

        /**************************************************
         * 登录应用程序处理函数
         * ***********************************************/
        private void ButtonLoginEventHandler(object sender, UserEventArgs e)
        {
            User user  = Database.GetUserByName(e.Username);
            if (null == user)
            {
                MessageBox.Show("User name is not exist. please try again","Login Fail",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 替换
            string _password = Utility.Encrypt(e.Password);

            if (e.Password != user.Password)
            {
                MessageBox.Show("Password error, please try again", "Login Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Main mainFrom = new Main();
            mainFrom.Show();
            this.Close();
        }

        /**************************************************
         * 退出应用程序处理函数
         * ***********************************************/
        private void ButtonExitEventHandler(object sender, UserEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UserLoginView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);  
            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(this);

            // 如果鼠标位置在窗体内，允许拖动  
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (position.X >= 0 && position.X < this.ActualWidth && position.Y >= 0 && position.Y < this.ActualHeight)
                {
                    this.DragMove();
                }
            }  
        }

    }
}

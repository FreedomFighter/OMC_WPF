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

namespace omc.src
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            ToolBar.ButtonExitClickEvent += new ButtonClickEventHandler(ButtonExitEventHandler);
        }

        private void ButtonExitEventHandler(object sender, EventArgs e)
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

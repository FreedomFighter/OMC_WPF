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
    public delegate void ButtonClickEventHandler(object sender, EventArgs e);

    /// <summary>
    /// ToolBar.xaml 的交互逻辑
    /// </summary>
    public partial class ToolBar : UserControl
    {
        public event ButtonClickEventHandler ButtonExitClickEvent = null;
       
        public ToolBar()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit application?", "Exit Applicatoin", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (null != ButtonExitClickEvent)
                {
                    ButtonExitClickEvent(BtnExit, new EventArgs());
                }
            }
        }
    }
}

﻿using System;
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
    /// PortSettingWin.xaml 的交互逻辑
    /// </summary>
    public partial class PortSettingWin : Window
    {
        public PortSettingWin()
        {
            InitializeComponent();
        }
    }

    public class ComPort
    {
        public string PortName;
        public string Baudrate;
    }
}

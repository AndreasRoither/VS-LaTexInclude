﻿using GalaSoft.MvvmLight;
using LatechInclude.ViewModel;
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

namespace LatechInclude
{
    /// <summary>
    /// Interaction logic for SwitchViewWindow.xaml
    /// </summary>
    public partial class SwitchViewWindow : Window
    {
        public SwitchViewWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();       
        }
    }
}

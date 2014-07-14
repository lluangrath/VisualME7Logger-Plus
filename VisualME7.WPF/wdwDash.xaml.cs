using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VisualME7.WPF.classes;
using VisualME7.WPF.viewmodel;

namespace VisualME7.WPF
{
    /// <summary>
    /// Interaction logic for wdwGauges.xaml
    /// </summary>
    public partial class wdwDash : Window
    {
        public wdwDash()
        {
            InitializeComponent();
            this.DataContext = new DashboardViewModel();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml.Linq;
using ITP_StateMachine.Classes;
using ITP_StateMachine.Helpers;
using ITP_StateMachine.ViewModels;

namespace ITP_StateMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel main = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = main;
        }
        
    }
}

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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ITP_StateMachine.Classes;
using ITP_StateMachine.Helpers;
using ITP_StateMachine.IDTechReader;
using ITP_StateMachine.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ITP_StateMachine.Views
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        DispatcherTimer dispatcherTimer;
        TimeSpan time;
        EventRecordManager events = new EventRecordManager();
        MsmqHelper msmq = new MsmqHelper();


        public PreviewWindow()
        {
            
            InitializeComponent();

            DataContext = new PreviewViewModel();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartIdleTimer();

            time = TimeSpan.FromSeconds(5);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
            WindowChecker.WindowCheck = true;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (time == TimeSpan.Zero)
            {
                dispatcherTimer.Stop();
            }
            else
            {
                time = time.Add(TimeSpan.FromSeconds(-1));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowChecker.WindowCheck = true;
            CardDetails.CardNumber = "";

            msmq.SendHardwareQueue("Device search initialize");
            LogHelper.SendLogToText("Device search initialize");
            events.ReceiveHardwareQueue();

        }
        public void StartIdleTimer()
        {
            msmq.SendTimerQueue("Idle timer start");
            LogHelper.SendLogToText("Idle timer start");
            events.ReceiveTimerQueue(15, 0);
            msmq.SendCommandQueue("Preview window closed");
            LogHelper.SendLogToText("Preview window closed");
        }
    }
}

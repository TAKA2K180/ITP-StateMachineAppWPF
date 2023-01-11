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
        SingleQueueHelper singleQueue = new SingleQueueHelper();
        PreviewViewModel preview = new PreviewViewModel();


        public PreviewWindow()
        {
            
            InitializeComponent();

            DataContext = preview;
               
            PreviewViewModel.CloseAction = new Action(Exit);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartIdleTimer();

            time = TimeSpan.FromSeconds(5);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
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
            MainWindow instance = Application.Current.Windows.OfType<MainWindow>().SingleOrDefault();
            if (instance == null)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                //singleQueue.SendToQueue("Main window show");
                //LogHelper.SendLogToText("Main window show");
            }

            
        }
        public void StartIdleTimer()
        {
            singleQueue.SendToQueue("Idle timer start");
            LogHelper.SendLogToText("Idle timer start");
            events.ReceiveQueue();
            singleQueue.SendToQueue("Preview window closed");
        }

        public void Exit()
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CardDetails.PrevCardNumber = null;
            CardDetails.CardNumber = null;
            CardDetails.PrevCardId = 0;
            CardDetails.CorpId = 0;
        }
    }
}

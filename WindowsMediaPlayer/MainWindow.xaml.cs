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
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
using System.Windows.Shell;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.ComponentModel;
using System.Reflection;

namespace WindowsMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> source = new List<string>();
        DispatcherTimer timer = new DispatcherTimer();
        TaskbarManager taskObj = TaskbarManager.Instance;
        BitmapImage img;
        public MainWindow()
        {
            InitializeComponent();

            Icon=new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\app.ico"));
            pause.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+"\\pause.png"));
            soundMuted.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\volMute.png"));
            forward.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\forw.png"));
            backward.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\forw.png"));
            play.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\play.png"));
            stop.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\stopB.png"));
            sound.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\volUp.png"));
            Mediaimg.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\pm.png"));
            del.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+ "\\del.png"));
            open.Source= new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\open.png"));


            this.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();
            this.play.Visibility = Visibility.Hidden;
            this.soundMuted.Visibility = Visibility.Hidden;               


            timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timer_Tick;
            //timer.Start();          
        }

        private void timer_Tick(object sender, EventArgs e)
        {
        if ((this.media.Source != null) && (this.media.NaturalDuration.HasTimeSpan))
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            progressBar.Value = media.Position.TotalSeconds;
            lbl.Content = String.Format("{0} / {1}", media.Position.ToString(@"mm\:ss"), media.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

            TaskbarManager taskbarInstance = TaskbarManager.Instance;
            taskbarInstance.SetProgressState(TaskbarProgressBarState.Normal);
            taskbarInstance.SetProgressValue((int)media.Position.TotalSeconds, (int)media.NaturalDuration.TimeSpan.TotalSeconds);

            }
        }
    private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.ValidateNames = true;
                dialog.Filter = "MP4|*.mp4|MP3|*.mp3|WMV|*.wmv|WAV|*.wav";
                bool? dialog_Ok = dialog.ShowDialog();
                if (dialog_Ok == true)
                {
                    foreach (string name in dialog.FileNames)
                    {
                    string[] st = name.Split('\\');
                    this.itemsList.Items.Add(st[st.Length-1]);
                    source.Add(name);
                    }
                MainWin.Title = this.itemsList.Items.GetItemAt(0).ToString();             
                media.Source = new Uri(source[0]);                  
                }
                media.LoadedBehavior = MediaState.Manual;
                media.UnloadedBehavior = MediaState.Manual;               
                media.Play();             
                checkType();
                
        }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("Choose a file first");
            }
}
        public void checkType()
        {
            if (MainWin.Title.Substring(MainWin.Title.Length - 4, 4) == ".mp4")
            {
                Mediaimg.Visibility = Visibility.Hidden;
                img = new BitmapImage();
                img.BeginInit();               
                img.UriSource = new Uri(Directory.GetCurrentDirectory() + "\\mp4.ico");
                img.EndInit();
                MainWin.TaskbarItemInfo.Overlay = img;
            }
            if (MainWin.Title.Substring(MainWin.Title.Length - 4, 4) == ".mp3")
            {
                img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri("C:\\Users\\Dina\\Desktop\\icons\\ico\\mp3.ico");
                img.EndInit();
                MainWin.TaskbarItemInfo.Overlay = img;
            }
            if (MainWin.Title.Substring(MainWin.Title.Length - 4, 4) == ".wav")
            {
                img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri("C:\\Users\\Dina\\Desktop\\icons\\ico\\wav.ico");
                img.EndInit();
                MainWin.TaskbarItemInfo.Overlay = img;
            }           
        }

        private void play_MouseUp(object sender, MouseButtonEventArgs e)
        {
            media.Play();
            taskObj.SetProgressState(TaskbarProgressBarState.Normal);
            play.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Visible; 
        }
        private void pause_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.media.Pause();
            taskObj.SetProgressState(TaskbarProgressBarState.Paused);
            this.pause.Visibility = Visibility.Hidden;
            this.play.Visibility = Visibility.Visible;
        }

        private void sound_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.media.Volume = 0;
            this.volSlider.Value = 0;
            this.sound.Visibility = Visibility.Hidden;
            this.soundMuted.Visibility = Visibility.Visible;
        }

        private void soundMuted_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.media.Volume = 50;
            this.volSlider.Value = 50;
            this.soundMuted.Visibility = Visibility.Hidden;
            this.sound.Visibility = Visibility.Visible;
        }

        private void volSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.media.Volume = this.volSlider.Value;
            if(this.volSlider.Value == 0)
            {
                this.sound.Visibility = Visibility.Hidden;
                this.soundMuted.Visibility = Visibility.Visible;
            }
            else if (this.volSlider.Value > 0 && this.sound.Visibility == Visibility.Hidden)
            {
                this.soundMuted.Visibility = Visibility.Hidden;
                this.sound.Visibility = Visibility.Visible;               
            }
        }

        private void forward_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.media.Position = media.Position.Add(new TimeSpan(0, 0, 15));            
        }
        private void backward_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.media.Position = media.Position.Add(new TimeSpan(0, 0, -10));
        }

        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.media.Position = TimeSpan.FromSeconds(progressBar.Value);
        }

        private void itemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
        }
        public void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
          
            this.TaskbarItemInfo.ProgressValue = (double)e.ProgressPercentage / 100;
        }
        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan sec = this.media.NaturalDuration.TimeSpan;
            this.progressBar.Maximum = sec.TotalSeconds;
            timer.Start();
         
            int maxProgressbarValue =(int)media.NaturalDuration.TimeSpan.TotalSeconds;
            //JumpTask task = new JumpTask
            //{
            //    Title = "Check for Updates",
            //    Arguments = "/play",
            //    Description = "play media",
            //    CustomCategory = "Actions",
            //    IconResourcePath = Assembly.GetEntryAssembly().CodeBase,
            //    ApplicationPath = Assembly.GetEntryAssembly().CodeBase
            //};            
            // System.Windows.Shell.JumpList jlist = new System.Windows.Shell.JumpList();
            //JumpItem j= jlist.JumpItems.First();
            //j.CustomCategory = "j";
            //jlist.JumpItems.Add(task);
            //jlist.JumpItems.Add(j);
            //System.Windows.Shell.JumpList.SetJumpList(App.Current, jlist);
        }

        private void del_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.itemsList.Items.Remove(this.itemsList.SelectedItem);
        }

        private void itemsList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
        }
        private void itemsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in source)
            {
                var i = item.Split('\\');
                if (i[i.Length - 1].ToString() == itemsList.SelectedItem.ToString())
                {
                    media.Source = new Uri(item.ToString());
                }
            }
           
            this.MainWin.Title = this.itemsList.SelectedItem.ToString();
            if (this.MainWin.Title.Substring(this.MainWin.Title.Length - 4, 4) == ".mp4")
            {
                this.Mediaimg.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Mediaimg.Visibility = Visibility.Visible;
            }
            checkType();
        }

        private void speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.media.SpeedRatio = (double)speed.Value;
        }
        private void stop_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.media.Stop();
            this.play.Visibility = Visibility.Visible;
            this.pause.Visibility = Visibility.Hidden;            
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;


namespace MMP
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private string patch_music = null;

        double music_duration = 0;

        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        TimeSpan current_time;
        TimeSpan total_time;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_path_music_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();

            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                lbl_info_music.Text = this.patch_music = dialog.FileName;
            }

        }

       
        private async void btn_play_music_Click(object sender, RoutedEventArgs e)
        {

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();

            }
            if (this.patch_music != null)
            {
                audioFile = new AudioFileReader(this.patch_music);

                this.music_duration = audioFile.TotalTime.TotalSeconds;

                lbl_time.Text = "00.00 / " + (this.music_duration / 60).ToString();

                outputDevice.Init(audioFile);
            }
            outputDevice.Play();

            var progress = new Progress<int>(value => 
            {
                slider_progress.Value = value;

            });

            var progress_time = new Progress<int>(value =>
            {
                current_time = TimeSpan.FromSeconds(value);
                total_time = TimeSpan.FromMinutes(audioFile.TotalTime.TotalMinutes);

                lbl_time.Text = current_time.ToString(@"mm\:ss") + " / " + total_time.ToString(@"mm\:ss");

            });

            await Task.Run(() => progress_report(Convert.ToInt32(this.music_duration), progress, progress_time));
        }

        void progress_report(int music_durantion_seconds, IProgress<int> progress_status, IProgress<int> progress_status_seconds)
        {
            for (int i = 0; i < music_durantion_seconds; i++)
            {
                Thread.Sleep(1000);
                var status = (i * 100) / music_durantion_seconds;

                progress_status.Report(status);

                progress_status_seconds.Report(Convert.ToInt32(audioFile.CurrentTime.TotalSeconds));

            }
        }
    }
}

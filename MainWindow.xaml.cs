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
using System.Threading;

namespace Reaction
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private Stopwatch _stopwatch = new Stopwatch();
        private CancellationTokenSource _tokenSource;
        private bool _isEndClicked { get; set; }
        private long _validTimes { get; set; } = 0;
        private long _totalScores { get; set; } = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void showScores()
        {
            if(_validTimes == 0)
            {
                labelValid.Content = "0";
                labelAverage.Content = "";
            } else
            {
                labelValid.Content = _validTimes.ToString();
                labelAverage.Content = (_totalScores / _validTimes).ToString();
            }
        }

        private async Task TaskDelay(CancellationToken token)
        {
            int waiting_time = _random.Next(3000, 6000);
            _tokenSource.Token.ThrowIfCancellationRequested();
            await Task.Delay(waiting_time, token);
        }

        private async void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;
            _isEndClicked = false;
            _tokenSource = new CancellationTokenSource();
            try
            {
                await TaskDelay(_tokenSource.Token);
            }
            catch(TaskCanceledException)
            {
                labelTimeUsed.Content = "Don't cheat yourself!";
            }
            if (!_isEndClicked)
            {
                gridBackground.Background = Brushes.Red;
                _stopwatch.Start();
            }
        }

        private void buttonEnd_Click(object sender, RoutedEventArgs e)
        {
            _isEndClicked = true;
            if(_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                labelTimeUsed.Content = "Time used: " + _stopwatch.ElapsedMilliseconds.ToString() + " ms";
                _validTimes += 1;
                _totalScores += _stopwatch.ElapsedMilliseconds;
                showScores();
                _stopwatch.Reset();
                gridBackground.Background = Brushes.White;
            }
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }
            buttonStart.IsEnabled = true;
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            _validTimes = 0;
            _totalScores = 0;
            showScores();
        }
    }
}

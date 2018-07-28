using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWCBridge.WPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ButtonCommitLabel => "コマンド起動";

        /// <summary>
        /// 標準出力
        /// </summary>
        private ObservableCollection<String> outputLines = new ObservableCollection<string>();
        /// <summary>
        /// 標準出力
        /// </summary>
        public ObservableCollection<String> OutputLines
        {
            get { return outputLines; }
            set
            {
                outputLines = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputLines)));
            }
        }
        /// <summary>
        /// コミット
        /// </summary>
        public void CommandCommit()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo()
                {
                    FileName = @"c:\windows\system32\ipconfig.exe",
                    Arguments = @"/all",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(processStartInfo);
                OutputLines.Add("START");

                process.OutputDataReceived += Process_OutputDataReceived;
                process.BeginOutputReadLine();

                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.BeginErrorReadLine();

                process.Exited += Process_Exited;

                process.WaitForExit();
                OutputLines.Add("Exit");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Debug.WriteLine("Process_Exited");
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine("Process_ErrorDataReceived");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                OutputLines.Add(e.Data);
            }));
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine("Process_OutputDataReceived");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                OutputLines.Add(e.Data);
            }));
        }
    }
}

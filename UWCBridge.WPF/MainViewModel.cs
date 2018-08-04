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
        public string ButtonInterruptLabel => "Ctrl+C";

        /// <summary>
        /// コマンドプロセス
        /// </summary>
        Process process;

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
                // ワーキングディレクトリを指定しないと設定ファイルを読めないので注意
                string targetDirectory = @"D:\VSProjects\UWCBridge\UWCBridgeServer\bin\Debug\netcoreapp2.0\publish";
                string targetFileName = targetDirectory + @"\UWCBridgeServer.exe";
                string targetArguments = @"--urls http://localhost:5001";

                var processStartInfo = new ProcessStartInfo()
                {
                    WorkingDirectory= targetDirectory,
                    FileName = targetFileName,
                    Arguments = targetArguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                process = Process.Start(processStartInfo);
                OutputLines.Add("-------- START --------");

                process.OutputDataReceived += Process_OutputDataReceived;
                process.BeginOutputReadLine();

                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.BeginErrorReadLine();

                process.Exited += Process_Exited;
                process.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Debug.WriteLine("Process_Exited");
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                OutputLines.Add("-------- Process_Exited --------");
            }));
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
        /// <summary>
        /// 中断
        /// </summary>
        public void CommandInterrupt()
        {
            try
            {
                process?.Kill();
                process?.Dispose();
                process = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}

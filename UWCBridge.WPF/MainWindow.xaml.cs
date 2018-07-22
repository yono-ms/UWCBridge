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

namespace UWCBridge.WPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonExec_Click(object sender, RoutedEventArgs e)
        {
            string command = @"c:\windows\system32\ipconfig.exe";
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = command;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;

            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput.ReadToEnd();

            output = output.Replace("\r\r\n", "\n");
            TextBlockLog.Text = output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWCBridge.WPF
{
    public class MainViewModelSample : MainViewModel
    {
        public MainViewModelSample()
        {
            OutputLines.Add("サンプル文字列1");
            OutputLines.Add("サンプル文字列2");
            OutputLines.Add("サンプル文字列3");
            OutputLines.Add("サンプル文字列4");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace UIBrowser.VNREX.GlobalHotKey
{
    class VNREXHotKey
    {
        const int WindowsMessageHotkey = 786;
        public  static void RegisterHotKey()
        {
           var ghk = new GlobalHotkey(0, Keys.F2, new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle);
            if (!ghk.Register())
            {
                MessageBox.Show("F2热键注册失败");
            }
            else
            {
                MessageBox.Show("F2热键注册成功");
            }
            ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
            {
                //  判断是否热键消息
                if (Message.message == WindowsMessageHotkey)
                {
                    //  获取热键id
                    var id = Message.wParam.ToInt32();
                    Keys key = (Keys)(((int)Message.lParam >> 16) & 0xFFFF);
                    //  执行热键回调方法（执行时需要判断是否与注册的热键匹配）
                    string.Format("sdad");
                   //Instance.ExcuteHotKeyCommand(id);
                }
            };
        }
    }
}

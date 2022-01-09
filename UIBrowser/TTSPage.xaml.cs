
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIBrowser.VNREX.GlobalHotKey;

namespace UIBrowser
{
    /// <summary>
    /// TTSPage.xaml 的交互逻辑
    /// </summary>
    public partial class TTSPage : Page
    {
        public TTSPage()
        {
            InitializeComponent();
        }
        const int WindowsMessageHotkey = 786;
        private void SetCliSettingBtn_Click(object sender, RoutedEventArgs e)
        {

            var ghk = new GlobalHotkey(0, System.Windows.Forms.Keys.F2, new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle);
            if (!ghk.Register())
            {
            
                MessageBox.Show("F2热键注册失败");
       
            }
            else
            {
                ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
                {
                    //  判断是否热键消息
                    if (Message.message == WindowsMessageHotkey)
                    {
                        //  获取热键id
                        var id = Message.wParam.ToInt32();
                        System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)(((int)Message.lParam >> 16) & 0xFFFF);

                        //  执行热键回调方法（执行时需要判断是否与注册的热键匹配）
                        if(key == System.Windows.Forms.Keys.F2)
                        {
                            if (VNREX.Global.VoiceRoidCli.ini)
                            {
                                if(!VNREX.Global.TTSString.Equals(""))
                                VNREX.Global.VoiceRoidCli.proc.StandardInput.WriteLineAsync(VNREX.Global.TTSString);
                            }
                        }
                      
                        
                        //Instance.ExcuteHotKeyCommand(id);
                    }
                };
                SetCliSettingBtn.IsEnabled = false;
                MessageBox.Show("F2热键注册成功");
            }

        
        }

        private void SetCLIEXEPath_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog FD = new System.Windows.Forms.OpenFileDialog();
            FD.Filter = "All PE FILES | *.exe";
           
            System.Windows.Forms.DialogResult DR = FD.ShowDialog();
            if (DR == System.Windows.Forms.DialogResult.OK)
            {

                CliEXEPathTextBox.Text = FD.FileName;
            }
        }

        private void LoadCLIBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CliEXEPathTextBox.Text.Equals("")){
                MessageBox.Show("请选择CLI程序");
                return;
            }
          
            if (!VNREX.Global.VoiceRoidCli.ini)
            {
                try
                {
                    VNREX.Global.VoiceRoidCli.VoiceRoid2CliInit(CliEXEPathTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (VNREX.Global.VoiceRoidCli.ini)
                {
                    //VNREX.Global.VoiceRoidCli.proc.StandardInput.WriteLineAsync("StartService");
                    System.Threading.Thread.Sleep(500);
                    TTSConsoleTextBox.AppendText("\r\n");
                    TTSConsoleTextBox.AppendText("初始化成功请点击注册热键按F2将对HOOK到的文本进行语音合成");

                    LoadCLIBtn.IsEnabled = false;
                }
                else
                {
                    TTSConsoleTextBox.AppendText("\r\n");
                    TTSConsoleTextBox.AppendText("初始化失败");
                }

            }
        }

        private void TTSConsoleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TTSConsoleTextBox.ScrollToEnd();
        }
    }
}

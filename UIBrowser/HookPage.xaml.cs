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
using UIBrowser.VNREX;
using System.Diagnostics;
using System.Threading;
using GameHook.TextHook;
using System.Text.RegularExpressions;
namespace UIBrowser
{
    /// <summary>
    /// HookPage.xaml 的交互逻辑
    /// </summary>
    public partial class HookPage : Page
    {
        public HookPage()
        {
            InitializeComponent();
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            VNREX.Global.taskProcess.Clear();
            try
            {
                processSelect.Items.Clear();
                Process[] processes = Process.GetProcesses();
                //只遍历任务任务栏显示的进程
                for (int i = 0; i < processes.Length; i++)
                {
                    if (processes[i].MainWindowTitle.Length > 0)
                    {
                        VNREX.Global.taskProcess.Add(processes[i]);

                    }
                }
                //从任务栏应用中找出多进程 
                for (int m = 0; m < processes.Length; m++)
                {
                    for (int j = 0; j < VNREX.Global.taskProcess.Count; j++)
                    {

                        if (processes[m].ProcessName.Equals(VNREX.Global.taskProcess[j].ProcessName) && processes[m].Id != VNREX.Global.taskProcess[j].Id)
                        {
                            bool isEqual = false;
                            for (int h = 0; h < VNREX.Global.taskProcess.Count; h++)
                            {
                                if (processes[m].Id == VNREX.Global.taskProcess[h].Id)
                                    isEqual = true;
                            }
                            if (!isEqual)
                                VNREX.Global.taskProcess.Add(processes[m]);
                        }
                    }
                }
                //排序
                VNREX.Global.taskProcess.Sort(delegate (Process x, Process y) {
                    return string.Compare(x.MainModule.ModuleName, y.MainModule.ModuleName);
                });


                //添加任务栏上的进程到combox中
                for (int k = 0; k < VNREX.Global.taskProcess.Count; k++)
                {
                    string processInfo = string.Format("PID:{0} ProcessName:{1}", VNREX.Global.taskProcess[k].Id, VNREX.Global.taskProcess[k].MainModule.ModuleName);
                    processSelect.Items.Add(processInfo);
                }
            }
            catch(Exception exception)
            {

                MessageBox.Show("请以管理员权限打开本程序！"+ exception.Message);
            }
   
        }
        /// <summary>
        /// 通过查询模块是否有WOW64.DLL来确定是否为32位进程
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public Boolean isWow64Application(Process process)
        {
            Boolean isWOW64 = false;
            for (int i = 0; i < process.Modules.Count; i++)
            {
                if (process.Modules[i].ModuleName.Equals("wow64.dll"))
                {
                    isWOW64 = true;
                    break;
                }
            }
            return isWOW64;
        }
        /// <summary>
        /// 特殊码更新时的处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hookInfo"></param>
        private void OnAddHcode(object sender, HookInfo hookInfo)
        {

            VNREX.Global.HcodeGroup.Add(hookInfo.FullDescriptCode);

        }
        private void UpdateConsoleTextBoxEvent(TextBox tb, HookInfo hookInfo)
        {
            if (consoleTextBox.Text.Length > 10000)
                consoleTextBox.Clear();
            //如果文本来自选择的特殊码就翻译加入文本框如果 不是就不翻译
            if (TextTractorHCODEData.SelectedIndex == hookInfo.index)
            {
                VNREX.Global.isTranslateText = true;

                consoleTextBox.AppendText("\r\n");
                //去重
                if (VNREX.Global.isFilterRepeat && hookInfo.OriginText.Length % VNREX.Global.repeatCount == 0)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hookInfo.OriginText.Length; ++i)
                    {
                        if ((i % VNREX.Global.repeatCount) == 0)
                        {
                            builder.Append(hookInfo.OriginText.Substring(i, 1));
                        }
                    }
                    hookInfo.OriginText = builder.ToString();
                }

                //如果使用了正则表达式就过滤文本
                if (!VNREX.Global.UpadateRegexString.Equals(""))
                {
                    // (?s)(.)(?=.*\\1) 叠词替换为空  (?s) 开启单行模式 DOTALL 让. 号匹配任意字符   (.) 任意字符 并捕获在第一组  (?=.*\1) 这是断言, 表示后面内容将是 任意个字符加上第一组所捕获的内容 
                    //这样子,如果这整个式子匹配到,表示,第一个捕获组内容在字符串中,至少出现两次，替换为 "" 空串.  进行 全局替换后， 整个字符串所出现的字符将不重复。
                    //原始网址https://www.jb51.net/article/25523.htm
                    string replaceText = Regex.Replace(hookInfo.OriginText, VNREX.Global.UpadateRegexString, "");

                    hookInfo.OriginText = replaceText;
                }
                consoleTextBox.AppendText(hookInfo.OriginText);
                VNREX.Global.TTSString = hookInfo.OriginText;
                //执行翻译 
                VNREX.Global.TranslateInstance.Translate(hookInfo.OriginText, VNREX.Global.LangSetting);
            }
            else
            {
                VNREX.Global.isTranslateText = false;
            }
        }
        /// <summary>
        /// 文本更新时的处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hookInfo"></param>
        private void OnUpadeteText(object sender, HookInfo hookInfo)
        {
            if (hookInfo.OriginText.Equals(""))
                return;
            Action<TextBox, HookInfo> updateAction = new Action<TextBox, HookInfo>(UpdateConsoleTextBoxEvent);
            consoleTextBox.Dispatcher.BeginInvoke(updateAction,consoleTextBox, hookInfo);

      
          
            if (!VNREX.Global.isTranslateText)
                return;

            
  
        }
        private void UpdateBtnStatus(Button tb, bool con)
        {
            tb.IsEnabled = con;
       
        }
        private void UpdateConsoleOutPutEvent(TextBox tb, HookInfo str)
        {
            tb.AppendText("\r\n");
            tb.AppendText(str.OriginText);
        }
        /// <summary>
        /// texttracotr console主机输出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hookInfo"></param>
        private void OnConsoleUpdate(object sender, HookInfo hookInfo)
        {
            if (hookInfo.OriginText.Equals("Textractor: couldn't inject"))
            {
                Action<Button, bool> updateAction = new Action<Button, bool>(UpdateBtnStatus);
                injectProcessBtn.Dispatcher.BeginInvoke(updateAction, injectProcessBtn, false);
                insertHCODEBtn.Dispatcher.BeginInvoke(updateAction, insertHCODEBtn, false);
         
            }
            Action<TextBox, HookInfo> updateConsoleOutputAction = new Action<TextBox, HookInfo>(UpdateConsoleOutPutEvent);
            consoleTextBox.Dispatcher.BeginInvoke(updateConsoleOutputAction, consoleTextBox, hookInfo);

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (!VNREX.Global.server.serverIsOpen)
                VNREX.Global.server.startServer();
            if (this.processSelect.SelectedIndex == -1)
                return;
            injectProcessBtn.IsEnabled = false;
            
            if (VNREX.Global.textHook.isInitialize)
            {
                VNREX.Global.textHook.HookGame(VNREX.Global.taskProcess[this.processSelect.SelectedIndex].Id);
            }
            else
            {
                //通过判断是否为64位进程来决定注入那个 32位还是64位texttrafctor
                VNREX.Global.textHook.isWOW64Process = isWow64Application(VNREX.Global.taskProcess[this.processSelect.SelectedIndex]);
                VNREX.Global.textHook.TextHookInit();
                if (VNREX.Global.textHook.isInitialize)
                {
                    VNREX.Global.textHook.updateHcode += OnAddHcode;
                    VNREX.Global.textHook.updateText += OnUpadeteText;
                    VNREX.Global.textHook.consoleUpdateText += OnConsoleUpdate;
              
                    Thread.Sleep(500);

                    VNREX.Global.textHook.HookGame(VNREX.Global.taskProcess[this.processSelect.SelectedIndex].Id);
                    //开启服务器等待连接 连接后将不停从队列取出数据发送过去;
                    VNREX.Global.textHook.gamePid = VNREX.Global.taskProcess[this.processSelect.SelectedIndex].Id;
                    insertHCODEBtn.IsEnabled = true;
                
                }
                else
                {
                    MessageBox.Show("初始化失败");
                }
            }
        }

        private void TextTractorHCODEData_DropDownOpened(object sender, EventArgs e)
        {
            TextTractorHCODEData.Items.Clear();
          
            foreach (string hcode in VNREX.Global.HcodeGroup)
            {
                TextTractorHCODEData.Items.Add(hcode);
            }
        }

        private void consoleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            consoleTextBox.ScrollToEnd();
        }

        private void insertHCODEBtn_Click(object sender, RoutedEventArgs e)
        {
          
            if (VNREX.Global.textHook.isInitialize || (!HOOKCODE.Text.Equals("")))
            {
                VNREX.Global.textHook.InsertHOOK(HOOKCODE.Text, VNREX.Global.textHook.gamePid);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void watchClipboard_Checked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.watchClipboard = true;
        }

        private void watchClipboard_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.watchClipboard = false;
        }

        private void openTranslateWindowBtn_Click(object sender, RoutedEventArgs e)
        {
           
            if (!VNREX.Global.translateWindow.isopen)
            {
                if (!VNREX.Global.server.serverIsOpen)
                    VNREX.Global.server.startServer();
                VNREX.Global.translateWindow.openTranslateWindow();

            }
            else
            {
                //5在客户端显示 翻译窗口
                String showWindow = "5:ShowTranslateWindow";
                VNREX.Global.server.safeCollectin.Enqueue(showWindow);
                
            }
        }

        private void RepeatCheckBo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RepeatCheckBo_Checked(object sender, RoutedEventArgs e)
        {

            VNREX.Global.isFilterRepeat = true;
            if (this.RepeatCountSelectComBox.SelectedIndex != -1)
            {
          
                VNREX.Global.repeatCount = int.Parse(this.RepeatCountSelectComBox.Items[this.RepeatCountSelectComBox.SelectedIndex].ToString()) + 1;
                
            }
        }

        private void UpdateRegexTextCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.UpadateRegexString= RegexTextBox.Text;
        }

        private void UpdateRegexTextCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.UpadateRegexString ="";
        }

        private void RepeatCheckBo_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.isFilterRepeat = false;
        }
    }
}

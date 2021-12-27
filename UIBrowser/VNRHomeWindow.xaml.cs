using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace UIBrowser
{

    /// <summary>
    /// VNRHomeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VNRHomeWindow : WindowX
    {
        public static HookPage hookPage = new HookPage();
        public static TranslationPage translationPage = new TranslationPage();
        public static OCRPage ocrPage = new OCRPage();
        public static TTSPage ttsPage = new TTSPage();
        public VNRHomeWindow()
        {
            InitializeComponent();
        }
        private void UpdateClipboardEvent()
        {
           
            if (Clipboard.ContainsData(DataFormats.Text))
            {
              
                string clibText = Clipboard.GetText();
                if (!string.IsNullOrEmpty(clibText))
                {
                    if (!VNREX.Global.preString.Equals(clibText))
                    {
                        VNREX.Global.preString = clibText;
                        VNREX.Global.TTSString = clibText;
                        VNREX.Global.TranslateInstance.Translate(clibText, VNREX.Global.LangSetting);
                        

                        hookPage.consoleTextBox.AppendText(clibText);
                        hookPage.consoleTextBox.AppendText("\r\n");
                    }

                }
            }
        }
        public void AutoTranslateProc(object obj)
        {
            while (true)
            {
               
                
                if (VNREX.Global.watchClipboard)
                {
                    Action updateAction = new Action(UpdateClipboardEvent);
                    hookPage.consoleTextBox.Dispatcher.BeginInvoke(updateAction);
       
                }
                System.Threading.Thread.Sleep(500);
            }
        }
        private void VNRHomeWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VNRContenFrame.Content = hookPage;
            //初始化百度API
            //开启一个线程池来 用来OCR自动翻译
            ThreadPool.QueueUserWorkItem(new WaitCallback(AutoTranslateProc));

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);  //打开配置文件
            var settings = configFile.AppSettings.Settings;
            if (settings["LangSetting"] == null)
                settings.Add("LangSetting", "日文到中文");

            if (settings["VoriceRoidCLIPath"] == null)
                settings.Add("VoriceRoidCLIPath", "");

            ttsPage.CliEXEPathTextBox.Text= settings["VoriceRoidCLIPath"].Value ;

            string langSetting = settings["LangSetting"].Value;
            if (langSetting.Equals("日文到中文")) {
                VNREX.Global.LangSetting = VNREX.LanguageSetting.ja2cn;
                translationPage.SelecteComboBox.SelectedIndex = 0;
            }
            if (langSetting.Equals("英文到中文"))
            {
                VNREX.Global.LangSetting = VNREX.LanguageSetting.en2cn;
                translationPage.SelecteComboBox.SelectedIndex = 1;

            }
            if (langSetting.Equals("韩文到中文"))
            {
                VNREX.Global.LangSetting = VNREX.LanguageSetting.kor2cn;
                translationPage.SelecteComboBox.SelectedIndex = 2;
            }


            if (settings["BaiDuAppID"] == null)
                settings.Add("BaiDuAppID", "");

            if (settings["BaiDuKey"] == null)
                settings.Add("BaiDuKey", "");

            if (settings["BaiDuEnable"] == null)
                settings.Add("BaiDuEnable", "false");

            VNREX.Global.BaiduAppid = settings["BaiDuAppID"].Value;
           VNREX.Global.BaiduKey = settings["BaiDuKey"].Value;
            VNREX.Global.BaiduEnable =  bool.Parse(settings["BaiDuEnable"].Value);
            if (VNREX.Global.BaiduEnable)
            {
                translationPage.BaiDuTranslateEnableCheckBox.IsChecked = true;
            }
            else
            {
                translationPage.BaiDuTranslateEnableCheckBox.IsChecked = false;
            }
            translationPage.BaiduAPPIDTextBox.Text = VNREX.Global.BaiduAppid;
            translationPage.BaiduKEYTextBox.Text = VNREX.Global.BaiduKey;
            //初始化腾讯API
            if (settings["TencentAppid"] == null)
                settings.Add("TencentAppid", "");

            if (settings["TencentKey"] == null)
                settings.Add("TencentKey", "");

            if (settings["TencentEnable"] == null)
                settings.Add("TencentEnable", "false");

            VNREX.Global.TencentAppid = settings["TencentAppid"].Value;
            VNREX.Global.TencentKey = settings["TencentKey"].Value;
            VNREX.Global.TencentEnable = bool.Parse(settings["TencentEnable"].Value);
            if (VNREX.Global.TencentEnable)
            {
                translationPage.TencentTranslateEnableCheckBox.IsChecked = true;
          
            }
            else
            {
                translationPage.TencentTranslateEnableCheckBox.IsChecked = false;

            }
            translationPage.TencentAPPIDTextBox.Text = VNREX.Global.TencentAppid;
            translationPage.TencentKEYTextBox.Text = VNREX.Global.TencentKey;



            if (settings["YouDaoEnable"] == null)
                settings.Add("YouDaoEnable", "false");
            VNREX.Global.YouDaoEnable = bool.Parse(settings["YouDaoEnable"].Value);
            if (VNREX.Global.YouDaoEnable)
            {

                translationPage.YouDaoTranslateEnableCheckBox.IsChecked = true;
            }
            else
            {
                translationPage.YouDaoTranslateEnableCheckBox.IsChecked = false;

            }
        }

        private void HookSettingBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VNRContenFrame.Content = hookPage;
        }

        private void TranslateAPISettingBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VNRContenFrame.Content = translationPage;
        }

        private void VNRHomeMainWindow_Closed(object sender, EventArgs e)
        {
            if (VNREX.Global.server.serverIsOpen)
                VNREX.Global.server.closeServer();
        }

        private void OCRSettingBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VNRContenFrame.Content = ocrPage;
        }

        private void TTSSettingBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VNRContenFrame.Content = ttsPage;
        }
    }
}

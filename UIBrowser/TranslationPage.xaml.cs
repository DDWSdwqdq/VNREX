using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace UIBrowser
{
    /// <summary>
    /// TranslationPage.xaml 的交互逻辑
    /// </summary>
    public partial class TranslationPage : Page
    {
        public TranslationPage()
        {
            InitializeComponent();
        }

        private void SaveSettingBtn_Click(object sender, RoutedEventArgs e)
        {




            try
            {

                //更新全局变量
                VNREX.Global.BaiduAppid = BaiduAPPIDTextBox.Text;
                VNREX.Global.BaiduKey = BaiduKEYTextBox.Text;
                VNREX.Global.TencentAppid = TencentAPPIDTextBox.Text;
                VNREX.Global.TencentKey = TencentKEYTextBox.Text;
                VNREX.Global.CaiYunToken = CaiYunTokenTextBox.Text;

                 var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);  //打开配置文件
                var settings = configFile.AppSettings.Settings;
                if (SelecteComboBox.SelectedIndex != -1)
                {
                    string setting = (SelecteComboBox.Items[SelecteComboBox.SelectedIndex] as string);

                    if (settings["LangSetting"] == null)
                        settings.Add("LangSetting", "日文到中文");

                    settings["LangSetting"].Value = setting;
                }

                if (settings["VoriceRoidCLIPath"] == null)
                    settings.Add("VoriceRoidCLIPath", "");
                settings["VoriceRoidCLIPath"].Value = UIBrowser.VNRHomeWindow.ttsPage.CliEXEPathTextBox.Text;

                if (settings["BaiDuAppID"] == null)
                    settings.Add("BaiDuAppID", "");

                if (settings["BaiDuKey"] == null)
                    settings.Add("BaiDuKey", "");

                if (settings["BaiDuEnable"] == null)
                    settings.Add("BaiDuEnable", "false");

                if (settings["TencentAppid"] == null)
                    settings.Add("TencentAppid", "");

                if (settings["TencentKey"] == null)
                    settings.Add("TencentKey", "");

                if (settings["TencentEnable"] == null)
                    settings.Add("TencentEnable", "false");

                if (settings["YouDaoEnable"] == null)
                    settings.Add("YouDaoEnable", "false");


                if (settings["CaiYunEnable"] == null)
                    settings.Add("CaiYunEnable", "false");

                if (settings["CaiYunToken"] == null)
                    settings.Add("CaiYunToken", "");

                settings["BaiDuAppID"].Value = VNREX.Global.BaiduAppid;
                settings["BaiDuKey"].Value = VNREX.Global.BaiduKey;
                settings["BaiDuEnable"].Value = VNREX.Global.BaiduEnable.ToString();

                settings["TencentAppid"].Value = VNREX.Global.TencentAppid;
                settings["TencentKey"].Value = VNREX.Global.TencentKey;
                settings["TencentEnable"].Value = VNREX.Global.TencentEnable.ToString();

                settings["YouDaoEnable"].Value = VNREX.Global.YouDaoEnable.ToString();

                settings["CaiYunToken"].Value = VNREX.Global.CaiYunToken;
                settings["CaiYunEnable"].Value = VNREX.Global.CaiYunEnable.ToString();

                configFile.Save(ConfigurationSaveMode.Modified);  //保存配置文件
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void BaiDuTranslateEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.BaiduEnable = true;
        }

        private void BaiDuTranslateEnableCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.BaiduEnable = false;
        }

        private void TencentTranslateEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.TencentEnable = true;
        }

        private void TencentTranslateEnableCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.TencentEnable = false;
        }

        private void SelecteComboBox_Selected(object sender, RoutedEventArgs e)
        {
     
        }

        private void SelecteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelecteComboBox.SelectedIndex != -1)
            {
                string setting = (SelecteComboBox.Items[SelecteComboBox.SelectedIndex] as string);
                if (setting.Equals("日文到中文"))
                {
                    VNREX.Global.LangSetting = VNREX.LanguageSetting.ja2cn;
                }
                
                if (setting.Equals("英文到中文"))
                {
                    VNREX.Global.LangSetting = VNREX.LanguageSetting.en2cn;
                }
               
                if (setting.Equals("韩文到中文"))
                {
                    VNREX.Global.LangSetting = VNREX.LanguageSetting.kor2cn;
                }
            }
        }

        private void YouDaoTranslateEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.YouDaoEnable = true;
        }

        private void YouDaoTranslateEnableCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.YouDaoEnable = false;
        }

        private void TEsSettingBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CaiYunTranslateEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.CaiYunEnable = true;
        }

        private void CaiYunTranslateEnableCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VNREX.Global.CaiYunEnable = false;
        }
    }
}

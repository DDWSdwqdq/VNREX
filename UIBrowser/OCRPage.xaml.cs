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

namespace UIBrowser
{
    /// <summary>
    /// OCRPage.xaml 的交互逻辑
    /// </summary>
    public partial class OCRPage : Page
    {
        public OCRPage()
        {
            InitializeComponent();
        }

        private void SelecteScreenAreaBtn_Click(object sender, RoutedEventArgs e)
        {
            VNREX.OCR.OCRServer.GenerateRect();
        }
        private void UpdateOCRConsoleEvent(string str)
        {
            if (OCRConsoleTextBox.Text.Length > 10000)
                OCRConsoleTextBox.Clear();
            OCRConsoleTextBox.AppendText("\r\n");
            OCRConsoleTextBox.AppendText(str);
        

        }
        public void UpdateOCRTextEvent(string msg)
        {
            Action<string> updateConsoleOutputAction = new Action<string>(UpdateOCRConsoleEvent);
            OCRConsoleTextBox.Dispatcher.Invoke(updateConsoleOutputAction, msg);
        }
        private void StartOCRBtn_Click(object sender, RoutedEventArgs e)
        {
          VNREX.OCR.OCRServer.updateText+= UpdateOCRTextEvent;
          if(VNREX.OCR.OCRServer.StartServer("8699"))
            {
                StartOCRBtn.IsEnabled = false;
                OpenOCRClientBtn.IsEnabled = true;
            }
        
        }

        private void OCRConsoleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OCRConsoleTextBox.ScrollToEnd();
        }

        private void OpenOCRClientBtn_Click(object sender, RoutedEventArgs e)
        {
            VNREX.OCR.OCRServer.OpenOCRClient();
        }
    }
}

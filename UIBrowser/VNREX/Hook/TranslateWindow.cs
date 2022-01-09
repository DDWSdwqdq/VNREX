using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace GameHook.TextHookWindwos
{
  public  class TranslateWindow
    {
        public Process TranslatorWindowProcess;
        public Boolean isopen;
        public TranslateWindow()
        {
            TranslatorWindowProcess = new Process();
            TranslatorWindowProcess.StartInfo.FileName = "WPFTranslatorWindow.exe";

        }
        public void openTranslateWindow()
        {
            String PriviousPath = Environment.CurrentDirectory;
            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "\\TranslatorWindow";
            try
            {
                Boolean result = TranslatorWindowProcess.Start();
                if (result)
                {
                    isopen = true;
                }
                else
                {
                    isopen = false;
                }

            }
            catch (Exception e)
            {
                isopen = false;

            }
            Environment.CurrentDirectory = PriviousPath;
        }
    }
}

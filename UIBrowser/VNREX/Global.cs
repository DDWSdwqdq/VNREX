using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  System.Diagnostics;
using UIBrowser.VNREX.NamePipe;
namespace UIBrowser.VNREX
{
   public static class Global
    {
     public static   List<Process> taskProcess=new List<Process>();
       public static List<String> HcodeGroup= new List<string>();
        public static Boolean isTranslateText = false;
        public static Boolean isFilterRepeat = false;
        public static int repeatCount = 2;
        public static string UpadateRegexString = "";
        public static bool watchClipboard = false;
        public static  NamePipe.NamePipeServer server = new NamePipe.NamePipeServer();
     public static GameHook.TextHook.TextHook textHook = new GameHook.TextHook.TextHook();
      public static  GameHook.TextHookWindwos.TranslateWindow translateWindow = new GameHook.TextHookWindwos.TranslateWindow();

        public static string BaiduAppid = "";
        public static string BaiduKey = "";
        public static bool BaiduEnable = false;
        public static string TencentAppid = "";
        public static string TencentKey = "";
        public static bool TencentEnable = false;
        public static string CaiYunToken = "";
        public static bool CaiYunEnable = false;


        public static bool YouDaoEnable = false;
        public static LanguageSetting LangSetting = LanguageSetting.ja2cn;

        public static TranslateController TranslateInstance = new TranslateController();

        public static string TTSString = "";
       public static string preString = "";
        public static TTS.VoiceRoid2 VoiceRoidCli = new TTS.VoiceRoid2();

    }
}

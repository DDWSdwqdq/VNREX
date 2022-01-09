using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GameHook.TextHook
{
    public class HookInfo:EventArgs
    {
        public Int64 ThreadHandle { get; set; }
        public int GameProcessID { get; set; }
        public String HookAddress { get; set; }
        public String HookCode { get; set; }
        public String SymbolName { get; set; }
        public String OriginText { get; set; }
        public String FullDescriptCode { get; set; }
        public int index { get; set; }

    }
    public delegate void TextEventHandler(object sender, HookInfo e);
  public class TextHook
    {
        private TextEventHandler updateHcodeEvent;
        public event TextEventHandler updateHcode
        {
            add
            {
                updateHcodeEvent += value;
            }
            remove
            {
                updateHcodeEvent -= value;
            }
        }
        private TextEventHandler updateTextEvent;
        public event TextEventHandler updateText
        {
            add
            {
                updateTextEvent += value;
            }
            remove
            {
                updateTextEvent -= value;
            }
        }
        //主机处理事件
        private TextEventHandler consoleTextEvent;
        public event TextEventHandler consoleUpdateText
        {
            add
            {
                consoleTextEvent += value;
            }
            remove
            {
                consoleTextEvent -= value;
            }
        }

        public List<HookInfo> hookDatas=new List<HookInfo>();
        public Queue<String> traractorHistory = new Queue<string>();
        private HookInfo tempHookInfo;
        public Process texttractorProcess;
        public Boolean isInitialize=false;
        public string textTractorLibPath = @"lib\TextTractor\";
        public Boolean _isX64Process = false;
        public string textTractorArcFilePath =@"x86";
        public int gamePid = 0;
        /// <summary>
        /// 是否为WOW64 32位进程
        /// </summary>
        public Boolean isWOW64Process {
            get { return _isX64Process; } 
            set {
                _isX64Process = value;
                if (value == true)
                {
                    textTractorArcFilePath = @"x86";
                   
                }
                else
                {
                    textTractorArcFilePath = @"x64";
                }
            }
        }
        /// <summary>
        /// 截取字符中间的内容
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public static string MidStrEx(string sourse, string startstr, string endstr)
         {
             string result = string.Empty;
             int startindex, endindex;
             try
             {
                 startindex = sourse.IndexOf(startstr);
                 if (startindex == -1)
                     return result;
                 string tmpstr = sourse.Substring(startindex + startstr.Length);
                 endindex = tmpstr.IndexOf(endstr);           
                 if (endindex == -1)
                     return result;
                 result = tmpstr.Remove(endindex);
             }
             catch (Exception ex)
             {
                 
             }
             return result;
         }    
        /// <summary>
        /// 解析texttractor标准输出 的字符
        /// </summary>
        /// <param name="outputString"></param>
        /// <returns></returns>
        private Boolean parseTextoOutData(string outputString)
        {

            if (outputString == null)
                return false;
            
            if (outputString.Equals(""))
            {
                return false;
            }
         //   Regex rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");//中括号[]
        //  string matchString=   rgx.Match(outputString).Value; //匹配的值可以作为子项添加到特殊码项目中
         string matchString = MidStrEx(outputString, "[", "]");
            string[] splitString=matchString.Split(':');
            if (splitString.Length == 7)
            {
                HookInfo hookInfo = new HookInfo();
                string originText=outputString.Replace("[" + matchString + "] ", "");
                hookInfo.GameProcessID = int.Parse(splitString[1], System.Globalization.NumberStyles.HexNumber);
                hookInfo.SymbolName = splitString[5];
                hookInfo.OriginText = originText;
                hookInfo.HookCode = splitString[6];
                hookInfo.HookAddress = splitString[2];
                hookInfo.FullDescriptCode = matchString;
                tempHookInfo = hookInfo;
                return true;
            }else if(splitString.Length == 9)
            {
                HookInfo hookInfo = new HookInfo();
                string originText = outputString.Replace("[" + matchString + "] ", "");
                hookInfo.ThreadHandle=Int64.Parse(splitString[0], System.Globalization.NumberStyles.HexNumber);
                hookInfo.GameProcessID = int.Parse(splitString[1], System.Globalization.NumberStyles.HexNumber);
                hookInfo.SymbolName = splitString[5];
                hookInfo.OriginText = originText;
                hookInfo.HookCode = splitString[6]+":"+splitString[7] + ":" + splitString[5];
                hookInfo.HookAddress = splitString[2];
                hookInfo.FullDescriptCode = matchString;
                tempHookInfo = hookInfo;
                return true;
            }else if(splitString.Length == 8)
            {
                HookInfo hookInfo = new HookInfo();
                string originText = outputString.Replace("[" + matchString + "] ", "");
                hookInfo.ThreadHandle = Int64.Parse(splitString[0], System.Globalization.NumberStyles.HexNumber);
                hookInfo.GameProcessID = int.Parse(splitString[1], System.Globalization.NumberStyles.HexNumber);
                hookInfo.SymbolName = splitString[5];
                hookInfo.OriginText = originText;
                hookInfo.HookCode = splitString[6] + ":" + splitString[7] + ":" + splitString[5];
                hookInfo.HookAddress = splitString[2];
                hookInfo.FullDescriptCode = matchString;
                tempHookInfo = hookInfo;
                return true;
            }
            else
            {
                return false;
            }
          
        }
        public void AddTextTraracorOutput(string output)
        {
            if (traractorHistory.Count > 1000)
            {
                traractorHistory.Dequeue();

            }
            traractorHistory.Enqueue(output);
        }
        /// <summary>
        /// texttractor输出处理
        /// </summary>
        /// <param name="sending"></param>
        /// <param name="arg"></param>
        void EventHandler(object sending,DataReceivedEventArgs arg)
        {
           
            AddTextTraracorOutput(arg.Data);
           
           Boolean parseStatus= parseTextoOutData(arg.Data);
            if (parseStatus)
            {
              
               if (!tempHookInfo.SymbolName.Equals("Console")&& !tempHookInfo.SymbolName.Equals("Clipboard") && !tempHookInfo.SymbolName.Equals(""))
                {
                    Boolean isExsits = false;
                    int i = 0;
                    //先判断特殊码是否已经存在不存在就添加到子项去
                    foreach(HookInfo info in hookDatas)
                    {
                    
                       if(info.HookCode.Equals(tempHookInfo.HookCode)&&info.ThreadHandle==tempHookInfo.ThreadHandle)
                        {
                            isExsits = true;
                            break;
                        }
                        else
                        {
                            isExsits = false;
                        }
                        i++;
                    }
                    if (isExsits)
                    {
                        tempHookInfo.index = i;
                        updateTextEvent.Invoke(this, tempHookInfo);
                    }
                    else
                    {
                        hookDatas.Add(tempHookInfo);
                          updateHcodeEvent.Invoke(this, tempHookInfo);
                    }
                     //注入失败时的处理事件
                }else if(tempHookInfo.SymbolName.Equals("Console"))
                {

                    consoleTextEvent.Invoke(this, tempHookInfo);
                }
            }
            else
            {

            }
        }
        /// <summary>
        /// 附加到游戏进程   /RS932#@004EC608 -P8612 注意需要先注入进程后才能插入特殊码不然程序报错
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task AttachGameProcess(int pid)
        {
            await texttractorProcess.StandardInput.WriteLineAsync("attach -P"+ pid);
            await texttractorProcess.StandardInput.FlushAsync();
        }
        /// <summary>
        /// 插入一个特殊码到游戏     /RS932#@004EC608 -P8612 注意需要先注入进程后才能插入特殊码不然程序报错
        /// </summary>
        /// <param name="hCode"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task InsertHookCode(string hCode,int pid)
        {
           
            await texttractorProcess.StandardInput.WriteLineAsync(hCode+" -P" + pid);
            await texttractorProcess.StandardInput.FlushAsync();
        }

        public async void InsertHOOK(string hCode, int pid)
        {
            await InsertHookCode( hCode,  pid);
        }
        public async void HookGame(int pid)
        {
            await AttachGameProcess(pid);
        }
        public Boolean TextHookInit()
        {
            String PriviousPath = Environment.CurrentDirectory;

            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory+ textTractorLibPath+ textTractorArcFilePath;


            texttractorProcess = new Process();
            texttractorProcess.StartInfo.FileName= "TextractorCLI.exe";
           texttractorProcess.StartInfo.CreateNoWindow = true;
            texttractorProcess.StartInfo.UseShellExecute = false;
            texttractorProcess.StartInfo.StandardOutputEncoding = Encoding.Unicode;
            
                 texttractorProcess.StartInfo.RedirectStandardInput = true;
            texttractorProcess.StartInfo.RedirectStandardOutput = true;//缓冲区
            
            texttractorProcess.OutputDataReceived += new DataReceivedEventHandler(EventHandler);
            try {
                texttractorProcess.Start();
                
            Environment.CurrentDirectory = PriviousPath;
                texttractorProcess.BeginOutputReadLine();
                
                                isInitialize = true;
                return true;
            }catch(Exception exception)
            {
                Environment.CurrentDirectory = PriviousPath;
                isInitialize = false;
                return false;
            }
        

        }

    
    }
}

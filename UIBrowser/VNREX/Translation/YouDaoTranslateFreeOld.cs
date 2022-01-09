
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace YouDao
{
    class YouDaoTranslateFreeOld
    {
        public enum TranslateLanguae
        {
            zh,
            en,
            ja,
            ko,
           
        }


        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">连接</param>
        /// <param name="headers">HTTP头</param>
        /// <param name="str">请求字符串</param>
        /// <returns>返回结果</returns>
        public static string PostInfOld(string url, WebHeaderCollection headers, string str)
        {
            //创建HTTP请求
            var re = WebRequest.Create(url) as HttpWebRequest;
            //设置请求头
            //re.Headers = headers;
            re.ContentType = "application/x-www-form-urlencoded";
            re.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            re.Referer = "https://www.lagou.com/jobs/list_unity3d?labelWords=&fromSearch=true&suginput=";
            //设置访问类型为POST
            re.Method = "POST";
            //写入请求信息
            using (StreamWriter sw = new StreamWriter(re.GetRequestStream()))
            {
                sw.WriteLine(str);
            }
            //获取相应内容
            var ans = re.GetResponse();
            using (var st = new StreamReader(ans.GetResponseStream()))
            {
                return st.ReadToEnd();
            }
        }
        /// <summary>
        /// 使用老版本不带_o的接口不用校对速度更快
        /// </summary>
        /// <param name="query"></param>
        /// <param name="languageFrom"></param>
        /// <param name="languageTo"></param>
        /// <returns></returns>
        public static string GetTranslate(string query,string languageFrom,string languageTo)
        {
            string trans = "";


                var url = "http://fanyi.youdao.com/translate?smartresult=dict&smartresult=rule";
                var headers = new WebHeaderCollection();
                headers["Content-Type"] = "application/x-www-form-urlencoded";
                headers["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
                headers["Referer"] = "https://www.lagou.com/jobs/list_unity3d?labelWords=&fromSearch=true&suginput=";
             string querytemp= query.Replace("…", "");
              var dict = new Dictionary<string, string>()
                            {
                                {"i",querytemp },
                                {"from",languageFrom },
                                {"to",languageTo },
                                {"smartresult","dict" },
                                {"client", "fanyideskweb" },
                                {"salt","15808837717114" },
                                {"sign","22a6ee9a07d4821f04e50bd029f73de0" },
                                {"ts" ,"1580883771711" },
                                {"bv","38c3ccbde2d50a86f0f9606d2be5a3d8"},
                                {"doctype","json" },
                                {"version","2.1"},
                                {"keyfrom","fanyi.web"},
                                {"action","FY_BY_REALTlME" }
                                
                            };
                var str = PostInfOld(url, headers, GetPostArgs(dict));
                //返回的是JSON格式 使用JSON解析它
                JObject jo = (JObject)JsonConvert.DeserializeObject(str);
                string erroCode = jo["errorCode"].ToString();
                if (!erroCode.Equals("0"))
                    return query;
                JObject temp;
                JArray array = JArray.Parse(jo["translateResult"].ToString());
            StringBuilder builder = new StringBuilder();
                for (int i = 0; i < array.Count; i++)
                {
                    JArray Joexhibitor = JArray.Parse(array[i].ToString());
                   for(int k=0;k< Joexhibitor.Count; k++)
                {
                    temp = JObject.Parse(Joexhibitor[k].ToString());
                    trans = temp["tgt"].ToString();
                    builder.Append(trans);
                }
               
                }
            trans = builder.ToString();
                return trans;
          

            
        }
        /// <summary>
        /// MD5　32位加密小写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5StrLower(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
                // To force the hex string to lower-case letters instead of
                // upper-case, use he following line instead:
                // sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
      
        public static string GetTranslateByOld(string query, TranslateLanguae languageFrom, TranslateLanguae languageTo)
        {
            string trans = "";


            var url = @"http://fanyi.youdao.com/translate?smartresult=dict&smartresult=rule";
            var headers = new WebHeaderCollection();
            headers["Content-Type"] = @"application/x-www-form-urlencoded; charset=UTF-8";
            headers["user-agent"] = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Mobile Safari/537.36";
            headers["Referer"] = @"https://fanyi.youdao.com/";
            string querytemp = query.Replace("…", "");

            //获取同java gettime()一样的 长整型时间
            long time = (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000;
            string lts = time.ToString();

            Random rd = new Random();
            int random = rd.Next(9);
            string salt = lts + (random).ToString();
            string bv = GetMD5StrLower(@"5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Mobile Safari/537.36");
            //注意使用小写  FY_BY_REALTIME FY_BY_CLICKBUTTION
            string sign = GetMD5StrLower(@"fanyideskweb" + querytemp + salt + @"Y2FYu%TNSbMCxc3t2u^XT");
            var dict = new Dictionary<string, string>()
                            {
                                {"i",    HttpUtility.UrlEncode(querytemp, Encoding.UTF8)},
                                {"from",languageFrom.ToString() },
                                {"to",languageTo.ToString() },
                                {"smartresult","dict" },
                                {"client", "fanyideskweb" },
                                {"salt",salt },
                                {"sign",sign},
                                {"lts" ,lts },
                                {"bv",bv},
                                {"doctype","json" },
                                {"version","2.1"},
                                {"keyfrom","fanyi.web"},
                                {"action","FY_BY_REALTlME" }

                            };
           //var jsonText= Post(url, dict);
            var str = PostInfOld(url, headers, GetPostArgs(dict));
            //  var str = PostInf(url, headers, GetPostArgs(dict));
            //返回的是JSON格式 使用JSON解析它
            JObject jo = (JObject)JsonConvert.DeserializeObject(str);
            string erroCode = jo["errorCode"].ToString();
            if (!erroCode.Equals("0"))
                return query;
            JObject temp;
            JArray array = JArray.Parse(jo["translateResult"].ToString());
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < array.Count; i++)
            {
                JArray Joexhibitor = JArray.Parse(array[i].ToString());
                for (int k = 0; k < Joexhibitor.Count; k++)
                {
                    temp = JObject.Parse(Joexhibitor[k].ToString());
                    trans = temp["tgt"].ToString();
                    builder.Append(trans);
                }

            }
            trans = builder.ToString();
            return trans;



        }

        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
           // req.ContentType = "application/x-www-form-urlencoded";

            req.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            req.UserAgent = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Mobile Safari/537.36";
            req.Referer = @"https://fanyi.youdao.com/";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

            public  static string GetTranslateMobile(string query, TranslateLanguae languageFrom, TranslateLanguae languageTo)
        {
            string trans = "";
            //KR2ZH_CN
            //EN2ZH_CN
            //JA2ZH_CN
            // var url = @"http://m.youdao.com/translate";
            string langType = "JA2ZH_CN";
            switch (languageFrom)
            {
                case TranslateLanguae.ja:
                    langType = "JA2ZH_CN";
                    break;
                case TranslateLanguae.en:
                    langType = "EN2ZH_CN";
                    break;
                case TranslateLanguae.ko:
                    langType = "KR2ZH_CN";
                    break;
                default:
                    break;
            }
            string querytemp = query.Replace("…", "");

            var dict = new Dictionary<string, string>()
                            {
                                {"inputtext","こんなことで、憧れのキャンパスライフを\n楽しめるだろうか……。%K%P"},
                                {"type","AUTO"},
                            };
            var html = Post(@"http://m.youdao.com/translate", dict);
            //返回的是JSON格式 使用JSON解析它
           // HtmlDocument doc = new HtmlDocument();//將回應的html流用 Html Agility Pack 解析
          //  doc.LoadHtml(html);//加载HTML代码
            
         //   HtmlNodeCollection htmlNodeTitle = doc.DocumentNode.SelectNodes("//*[@id='translateResult']/li");//返回群組            
           
                return "";
             
            

           // return htmlNodeTitle[0].InnerText;


        }
        /// <summary>
        /// 把字典转化为请求字符串
        /// </summary>
        /// <param name="dict">参数字典</param>
        /// <returns>返回请求字符串</returns>
        public static string GetPostArgs(Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var item in dict)
            {
                if (first)
                {
                    sb.Append(item.Key);
                    sb.Append("=");
                    sb.Append(item.Value);
                    first = false;
                }
                else
                {
                    sb.Append("&");
                    sb.Append(item.Key);
                    sb.Append("=");
                    sb.Append(item.Value);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">连接</param>
        /// <param name="headers">HTTP头</param>
        /// <param name="str">请求字符串</param>
        /// <returns>返回结果</returns>
        public static string PostInfMobile(string url, WebHeaderCollection headers, string str)
        {

            //创建HTTP请求
            var re = WebRequest.Create(url) as HttpWebRequest;
            //设置请求头
            //re.Headers = headers;
           // re.ContentType = @"application/x-www-form-urlencoded";
            re.UserAgent = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Mobile Safari/537.36";
            //re.Referer = @"http://m.youdao.com/translate";
            //设置访问类型为POST
            re.Method = "POST";
            //写入请求信息
            using (StreamWriter sw = new StreamWriter(re.GetRequestStream()))
            {
                sw.WriteLine(str);
            }
            //获取相应内容
            var ans = re.GetResponse();
            using (var st = new StreamReader(ans.GetResponseStream()))
            {
                return st.ReadToEnd();
            }
        }

    
    /// <summary>
    /// 发送Post请求
    /// </summary>
    /// <param name="url">连接</param>
    /// <param name="headers">HTTP头</param>
    /// <param name="str">请求字符串</param>
    /// <returns>返回结果</returns>
    public static string PostInf(string url, WebHeaderCollection headers, string str)
        {
         
            //创建HTTP请求
            var re = WebRequest.Create(url) as HttpWebRequest;
            //设置请求头
            //re.Headers = headers;
            re.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            re.UserAgent = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Mobile Safari/537.36";
            re.Referer = @"https://fanyi.youdao.com/";
            //设置访问类型为POST
            re.Method = "POST";
            //写入请求信息
            using (StreamWriter sw = new StreamWriter(re.GetRequestStream()))
            {
                sw.WriteLine(str);
            }
            //获取相应内容
            var ans = re.GetResponse();
            using (var st = new StreamReader(ans.GetResponseStream()))
            {
                return st.ReadToEnd();
            }
        }

    }
}

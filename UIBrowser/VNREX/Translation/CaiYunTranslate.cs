using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CaiYun
{
    class CaiYunTranslate
    {
        public enum Trans_Type
        {
            en2zh,
            ja2zh
        }
        public static async Task<string> PostSource(string url,string src,string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                HttpContent content = new StringContent(src);
                content.Headers.Add("X-Authorization", "token " + token);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //由HttpClient发出异步Post请求
                HttpResponseMessage res = await client.PostAsync(url, content);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string jsonStr = res.Content.ReadAsStringAsync().Result;

                    return jsonStr;
                }
                else
                    return "";
            }

        }
        public static string PostInfOld(string url, WebHeaderCollection headers, string str,string token)
        {




            //创建HTTP请求
            var re = WebRequest.Create(url) as HttpWebRequest;
            //设置请求头
            //re.Headers = headers;
            //re.Credentials = cc;
            // re.PreAuthenticate = true;
           
            re.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json").ToString();
            re.Headers.Add("X-Authorization", "token " + token);
           // re.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            //设置访问类型为POST
            re.Method = "POST";
           // re.PreAuthenticate = true;
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
        public static string GetTranslateByOld(string query, Trans_Type trans_type,string Token)
        {
            string trans = "";


            var url = @"http://api.interpreter.caiyunai.com/v1/translator";
            var headers = new WebHeaderCollection();
            headers["Content-Type"] = @"application/x-www-form-urlencoded; charset=UTF-8";
            headers["user-agent"] = @"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Mobile Safari/537.36";
            headers["Referer"] = @"https://fanyi.youdao.com/";
            string querytemp = query.Replace("…", "");

 
            var dict = new Dictionary<string, string>()
                            {
                                {"source",    querytemp},
                                {"trans_type",trans_type.ToString()},
                                {"request_id","demo" },
                                {"detect","dict" }
                            };
            var str = PostInfOld(url, headers, JsonConvert.SerializeObject(dict), Token);
           
            
            
              //返回的是JSON格式 使用JSON解析它
              JObject jo = (JObject)JsonConvert.DeserializeObject(str);
              string result = jo["target"].ToString();
                 return result;



        }
    }
}

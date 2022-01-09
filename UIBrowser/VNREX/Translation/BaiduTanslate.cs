using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class TranslationResult
{
    //错误码，翻译结果无法正常返回
    public string Error_code { get; set; }
    public string Error_msg { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Query { get; set; }
    //翻译正确，返回的结果
    //这里是数组的原因是百度翻译支持多个单词或多段文本的翻译，在发送的字段q中用换行符（\n）分隔
    public BaiduTranslation[] Trans_result { get; set; }


}
public class BaiduTranslation
{
    private string appId;
    private string key;
    private Language from;
    private Language to;
    public string Src { get; set; }
    public string Dst { get; set; }
    public enum Language
{
    //百度翻译API官网提供了多种语言，这里只列了几种
    auto = 0,
    zh = 1,
    en = 2,
    cht = 3,
    jp= 4,
     kor=5
    }
    public BaiduTranslation() { }
    /// <summary>
    /// 构造一个百度翻译类
    /// </summary>
    /// <param name="appid">百度申请的appiID</param>
    /// <param name="key">百度申请的KEY</param>
    /// <param name="from">源语言</param>
    /// <param name="to">到语言</param>
  public  BaiduTranslation(string appid,string key,Language from,Language to)
    {
        this.appId = appid;
        this.key = key;
        this.from = from;
        this.to = to;
    }
    private BaiduTranslation setAppID(string appid)
    {
        this.appId = appid;
        return this;
    }
    private BaiduTranslation setKey(string key)
    {
        this.key = key;
        return this;
    }

    //对字符串做md5加密
    private static string GetMD5WithString(string input)
    {
        if (input == null)
        {
            return null;
        }
        MD5 md5Hash = MD5.Create();
        //将输入字符串转换为字节数组并计算哈希数据  
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        //创建一个 Stringbuilder 来收集字节并创建字符串  
        StringBuilder sBuilder = new StringBuilder();
        //循环遍历哈希数据的每一个字节并格式化为十六进制字符串  
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        //返回十六进制字符串  
        return sBuilder.ToString();
    }

    /// <summary>
    /// 调用百度翻译API进行翻译
    /// 详情可参考http://api.fanyi.baidu.com/api/trans/product/apidoc
    /// </summary>
    /// <param name="q">待翻译字符</param>
    /// <param name="from">源语言</param>
    /// <param name="to">目标语言</param>
    /// <returns></returns>
     private TranslationResult GetTranslationFromBaiduFanyi(string q, Language from, Language to)
    {
        //可以直接到百度翻译API的官网申请
        //一定要去申请，不然程序的翻译功能不能使用


        string jsonResult = String.Empty;
        //源语言
        string languageFrom = from.ToString().ToLower();
        //目标语言
        string languageTo = to.ToString().ToLower();
        //随机数
        string randomNum = System.DateTime.Now.Millisecond.ToString();
        //md5加密
        string md5Sign = GetMD5WithString(this.appId + q + randomNum + this.key);
        //url
        string url = String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}",
            HttpUtility.UrlEncode(q, Encoding.UTF8),
            languageFrom,
            languageTo,
            this.appId,
            randomNum,
            md5Sign
            );
        WebClient wc = new WebClient();
        try
        {
            jsonResult = wc.DownloadString(url);
        }
        catch
        {
            jsonResult = string.Empty;
        }
        //解析json
        JavaScriptSerializer jss = new JavaScriptSerializer(); 
      
        TranslationResult result = jss.Deserialize<TranslationResult>(jsonResult);
        return result;
    }



    /// <summary>
    /// 将中文翻译为英文
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public  string translation(string source)
    {
        TranslationResult result = GetTranslationFromBaiduFanyi(source, from, to);
        if (result == null)
            return "";
        //判断是否出错
        if (result.Error_code == null)
        {
            return result.Trans_result[0].Dst;
        }
        else
        {
            return source;//如果 有错误就翻译原始文本
            //检查appid和密钥是否正确
          //  return "翻译出错，错误码：" + result.Error_code + "，错误信息：" + result.Error_msg;
        }
    }

    /// <summary>
    /// 将中文翻译为英文
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public string translationTeset(string source)
    {
        TranslationResult result = GetTranslationFromBaiduFanyi(source, from, to);
        //判断是否出错
        if (result.Error_code == null)
        {
            return result.Trans_result[0].Dst;
        }
        else
        {
           
            //检查appid和密钥是否正确
            return "翻译出错，错误码：" + result.Error_code + "，错误信息：" + result.Error_msg;
        }
    }

}




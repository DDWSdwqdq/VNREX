using System;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tmt.V20180321;
using TencentCloud.Tmt.V20180321.Models;
using System.Collections.Generic;

namespace Tencent
{
    public class TencentCloudTranslate
    {
        public enum TranslateLanguae
        {
            auto,
            zh,
            en,
            ja,
            ko
        }
        /// <summary>
        /// 腾讯API翻译测试
        /// </summary>
        /// <param name="sourceText">源始文本</param>
        /// <param name="sourceLanguage">原语言</param>
        /// <param name="targetLanguage">要到的翻译的语言</param>
        /// <param name="appID">SecretId</param>
        /// <param name="appKey">SecretKey</param>
        /// <returns>翻译结果</returns>
        public static string TranslateTest(String sourceText, String sourceLanguage, String targetLanguage,String appID,String appKey)
        {

            try
            {
                Credential cred = new Credential
                {
                    SecretId = appID,
                    SecretKey = appKey
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tmt.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                //注意这句非常重要 不加这句 程序会走await RequestV1(request, actionName);有可能出现签名错误的问题应该使用 await RequestV3(request, actionName);  这样程序就可以同一时间请求翻译
                clientProfile.SignMethod = ClientProfile.SIGN_TC3SHA256; 
                  //   public static string SIGN_TC3SHA256 = "TC3-HMAC-SHA256";
                TmtClient client = new TmtClient(cred, "ap-beijing", clientProfile);
                TextTranslateRequest req = new TextTranslateRequest();
                req.SourceText = sourceText;
                req.Source = sourceLanguage;

                req.ProjectId = 0;
                req.Target = targetLanguage;
                TextTranslateResponse resp = client.TextTranslateSync(req);
                
                //string jsonString = AbstractModel.ToJsonString(resp);
                return resp.TargetText;
              
            }
            catch (Exception e)
            {
                return e.InnerException.Message;
            }
        }

        /// <summary>
        /// 腾讯云翻译API
        /// </summary>
        /// <param name="sourceText">源始文本</param>
        /// <param name="sourceLanguage">原语言</param>
        /// <param name="targetLanguage">目标语言</param>
        /// <param name="appID">SecretId</param>
        /// <param name="appKey">SecretKey</param>
        /// <returns>翻译结果</returns>
        public static string TranslateSource(String sourceText, String sourceLanguage, String targetLanguage, String appID, String appKey)
        {
            // SecretId = "AKIDBCJAcrrcJkvpSosrTw9BwQecKm46s1f2",
            //      SecretKey = "GGzR2OWR1LUPq1vayOq2ze7KjriHZaLv"
            try
            {
                Credential cred = new Credential
                {
                    SecretId = appID,
                    SecretKey = appKey
                };

                ClientProfile clientProfile = new ClientProfile();
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tmt.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                //注意这句非常重要 不加这句 程序会走await RequestV1(request, actionName);有可能出现签名错误的问题应该使用 await RequestV3(request, actionName);  这样程序就可以同一时间请求翻译
                clientProfile.SignMethod = ClientProfile.SIGN_TC3SHA256;
                //   public static string SIGN_TC3SHA256 = "TC3-HMAC-SHA256";
                TmtClient client = new TmtClient(cred, "ap-beijing", clientProfile);
                TextTranslateRequest req = new TextTranslateRequest();
                req.SourceText = sourceText;
                req.Source = sourceLanguage;
                // req.ProjectId = 1257869118;
                req.ProjectId = 0;
                req.Target = targetLanguage;
                TextTranslateResponse resp = client.TextTranslateSync(req);

                //string jsonString = AbstractModel.ToJsonString(resp);
                return resp.TargetText;

            }
            catch (Exception e)
            {
                return sourceText;
            }
        }

    }
}

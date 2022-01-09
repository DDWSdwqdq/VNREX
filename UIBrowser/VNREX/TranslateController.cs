using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tencent;
namespace UIBrowser.VNREX
{
   public enum LanguageSetting
    {
        ja2cn,
        en2cn,
        kor2cn
    }
   public class TranslateController
    {
        public void Translate(string source,LanguageSetting setting)
        {
           // if (Global.BaiduEnable && (!Global.BaiduAppid.Equals("") && (!Global.BaiduKey.Equals(""))))
              if (Global.BaiduEnable && (!string.IsNullOrEmpty(Global.BaiduAppid)) && (!string.IsNullOrEmpty(Global.BaiduKey)))          
            {
                BaiduTranslation.Language from = BaiduTranslation.Language.jp;
                BaiduTranslation.Language to = BaiduTranslation.Language.zh;
                switch (setting)
                {
                    case LanguageSetting.ja2cn:
                        from= BaiduTranslation.Language.jp;
                        to = BaiduTranslation.Language.zh;
                        break;
                    case LanguageSetting.en2cn:
                        from = BaiduTranslation.Language.en;
                        to = BaiduTranslation.Language.zh;
                        break;
                    case LanguageSetting.kor2cn:
                        from = BaiduTranslation.Language.kor;
                        to = BaiduTranslation.Language.zh;
                        break;
                }
                Task.Run(() =>
                {
                    try
                    {
                        BaiduTranslation translater = new BaiduTranslation(Global.BaiduAppid, Global.BaiduKey, from, to);
                        var translateText = translater.translation(source);
                        translateText = translateText.Replace("\n", "");
                        translateText = translateText.Replace("\r", "");
                        string args = "1" + translateText;
                        Global.server.safeCollectin.Enqueue(args);
                    }
                    catch { }
                });


            }
           
            if (Global.TencentEnable && (!string.IsNullOrEmpty(Global.TencentAppid)) && (!string.IsNullOrEmpty(Global.TencentAppid)))
            {
                TencentCloudTranslate.TranslateLanguae from = TencentCloudTranslate.TranslateLanguae.ja;
                TencentCloudTranslate.TranslateLanguae to = TencentCloudTranslate.TranslateLanguae.zh;
                switch (setting)
                {
                    case LanguageSetting.ja2cn:
                        from = TencentCloudTranslate.TranslateLanguae.ja;
                        to = TencentCloudTranslate.TranslateLanguae.zh;
                        break;
                    case LanguageSetting.en2cn:
                        from = TencentCloudTranslate.TranslateLanguae.en;
                        to = TencentCloudTranslate.TranslateLanguae.zh;
                        break;
                    case LanguageSetting.kor2cn:
                        from = TencentCloudTranslate.TranslateLanguae.ko;
                        to = TencentCloudTranslate.TranslateLanguae.zh;
                        break;
                }
                Task.Run(() =>
                {
                    try {
                        string translateText = TencentCloudTranslate.TranslateSource(source, from.ToString(), to.ToString(), Global.TencentAppid, Global.TencentKey);
                        translateText = translateText.Replace("\n", "");
                        translateText = translateText.Replace("\r", "");
                        string args = "3" + translateText;
                        Global.server.safeCollectin.Enqueue(args);
                    } catch { }
                });
       
            }
            //      YouDao.YouDaoTranslateFreeOld s = new YouDao.YouDaoTranslateFreeOld();
            // var text = s.GetTranslateMobile("人Q", YouDao.YouDaoTranslateFreeOld.TranslateLanguae.ja, YouDao.YouDaoTranslateFreeOld.TranslateLanguae.zh);
            if (Global.YouDaoEnable)
            {
                YouDao.YouDaoTranslateFreeOld.TranslateLanguae from = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.ja;
                YouDao.YouDaoTranslateFreeOld.TranslateLanguae to = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.zh;
                switch (setting)
                {
                    case LanguageSetting.ja2cn:
                        from = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.ja;
                        to = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.zh;
                        break;
                    case LanguageSetting.en2cn:
                        from = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.en;
                        to = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.zh;
                        break;
                    case LanguageSetting.kor2cn:
                        from = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.ko;
                        to = YouDao.YouDaoTranslateFreeOld.TranslateLanguae.zh;
                        break;
                }
                Task.Run(() =>
                {
                    try
                    {
                        string translateText = YouDao.YouDaoTranslateFreeOld.GetTranslateByOld(source, from, to);
                        if (translateText.Equals(""))
                            return;
                      
                        translateText = translateText.Replace("\n", "");
                        translateText = translateText.Replace("\r", "");
                        string args = "2" + translateText;
                        Global.server.safeCollectin.Enqueue(args);
                    }
                    catch { }
                });

            }

            if (Global.CaiYunEnable && (!Global.CaiYunToken.Equals("")))
            {
                CaiYun.CaiYunTranslate.Trans_Type trans_type = CaiYun.CaiYunTranslate.Trans_Type.ja2zh;
  
                switch (setting)
                {
                    case LanguageSetting.ja2cn:
                        trans_type  = CaiYun.CaiYunTranslate.Trans_Type.ja2zh;
                        break;
                    case LanguageSetting.en2cn:
                        trans_type = CaiYun.CaiYunTranslate.Trans_Type.en2zh;
                        break;
                    case LanguageSetting.kor2cn:
                        trans_type = CaiYun.CaiYunTranslate.Trans_Type.en2zh;
                        break;
                }
                Task.Run(() =>
                {
                    try
                    {
                      var  translateText= CaiYun.CaiYunTranslate.GetTranslateByOld(source, trans_type,Global.CaiYunToken);

                        translateText = translateText.Replace("\n", "");
                        translateText = translateText.Replace("\r", "");
                        string args = "7" + translateText;
                        Global.server.safeCollectin.Enqueue(args);
                    }
                    catch { }
                });

            }

        }
    }
}

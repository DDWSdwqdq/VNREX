/*
 * Copyright (c) 2018 THL A29 Limited, a Tencent company. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace TencentCloud.Tmt.V20180321.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using TencentCloud.Common;

    public class SpeechTranslateResponse : AbstractModel
    {
        
        /// <summary>
        /// 请求的SessionUuid直接返回
        /// </summary>
        [JsonProperty("SessionUuid")]
        public string SessionUuid{ get; set; }

        /// <summary>
        /// 语音识别状态 1-进行中 0-完成
        /// </summary>
        [JsonProperty("RecognizeStatus")]
        public long? RecognizeStatus{ get; set; }

        /// <summary>
        /// 识别出的原文
        /// </summary>
        [JsonProperty("SourceText")]
        public string SourceText{ get; set; }

        /// <summary>
        /// 翻译出的译文
        /// </summary>
        [JsonProperty("TargetText")]
        public string TargetText{ get; set; }

        /// <summary>
        /// 第几个语音分片
        /// </summary>
        [JsonProperty("Seq")]
        public long? Seq{ get; set; }

        /// <summary>
        /// 原语言
        /// </summary>
        [JsonProperty("Source")]
        public string Source{ get; set; }

        /// <summary>
        /// 目标语言
        /// </summary>
        [JsonProperty("Target")]
        public string Target{ get; set; }

        /// <summary>
        /// 当请求的Mode参数填写bvad是，启动VadSeq。此时Seq会被设置为后台vad（静音检测）后的新序号，而VadSeq代表客户端原始Seq值
        /// </summary>
        [JsonProperty("VadSeq")]
        public long? VadSeq{ get; set; }

        /// <summary>
        /// 唯一请求 ID，每次请求都会返回。定位问题时需要提供该次请求的 RequestId。
        /// </summary>
        [JsonProperty("RequestId")]
        public string RequestId{ get; set; }


        /// <summary>
        /// For internal usage only. DO NOT USE IT.
        /// </summary>
        public override void ToMap(Dictionary<string, string> map, string prefix)
        {
            this.SetParamSimple(map, prefix + "SessionUuid", this.SessionUuid);
            this.SetParamSimple(map, prefix + "RecognizeStatus", this.RecognizeStatus);
            this.SetParamSimple(map, prefix + "SourceText", this.SourceText);
            this.SetParamSimple(map, prefix + "TargetText", this.TargetText);
            this.SetParamSimple(map, prefix + "Seq", this.Seq);
            this.SetParamSimple(map, prefix + "Source", this.Source);
            this.SetParamSimple(map, prefix + "Target", this.Target);
            this.SetParamSimple(map, prefix + "VadSeq", this.VadSeq);
            this.SetParamSimple(map, prefix + "RequestId", this.RequestId);
        }
    }
}


using COSXML;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hunter.Tencent.COSSDK
{
    public class CosXmlOptions
    {
        public CosXmlConfig Config { get; set; }
        public QCloudCredential QCloudCredential { get; set; }
        public string Appid { get; set; }
        public string Region { get; set; }

        /// <summary>
        /// 桶名称
        /// </summary>
        public string Bucket { get; set; }

    }

    /// <summary>
    /// 安全对象
    /// </summary>
    public class QCloudCredential
    {
        public string TmpSecretId { get; set; }
        public string TmpSecretKey { get; set; }
        public long DurationSecond { get; set; }
    }
}

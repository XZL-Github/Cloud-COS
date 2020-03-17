using COSXML.Auth;
using COSXML.Common;
using COSXML.CosException;
using COSXML.Log;
using COSXML.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hunter.Tencent.COSSDK
{
    public class TencentQCloudCredentialProvider : QCloudCredentialProvider
    {

        private string secretId;

        private string secretKey;

        private long keyDurationSecond;

        private string keyTime;

        public TencentQCloudCredentialProvider(string secretId, string secretKey, long keyDurationSecond)
            : this(secretId, secretKey, TimeUtils.GetCurrentTime(TimeUnit.SECONDS), keyDurationSecond)
        { }

        public TencentQCloudCredentialProvider(string secretId, string secretKey, long keyStartTimeSecond, long keyDurationSecond)
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.keyDurationSecond = keyDurationSecond;
            long keyEndTime = keyStartTimeSecond + keyDurationSecond;
            this.keyTime = String.Format("{0};{1}", keyStartTimeSecond, keyEndTime);
        }

        public override QCloudCredentials GetQCloudCredentials()
        {
            if (IsNeedUpdateNow()) Refresh();
            if (secretId == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "secretId == null");
            if (secretKey == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "secretKey == null");
            if (keyTime == null) throw new CosClientException((int)CosClientError.INVALID_CREDENTIALS, "keyTime == null");
            string signKey = DigestUtils.GetHamcSha1ToHexString(keyTime, Encoding.UTF8, secretKey, Encoding.UTF8);
            return new QCloudCredentials(secretId, signKey, keyTime);
        }

        public void SetSetQCloudCredential(string secretId, string secretKey, string keyTime)
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
            this.keyTime = keyTime;
        }

        public override void Refresh()
        {
            Console.WriteLine("Refresh");
            long keyStartTimeSecond = TimeUtils.GetCurrentTime(TimeUnit.SECONDS);
            long keyEndTime = keyStartTimeSecond + this.keyDurationSecond;
            SetSetQCloudCredential(secretId, secretKey, String.Format("{0};{1}", keyStartTimeSecond, keyEndTime));
        }

        public bool IsNeedUpdateNow()
        {
            if (String.IsNullOrEmpty(keyTime) || String.IsNullOrEmpty(secretId) || String.IsNullOrEmpty(secretKey))
            {
                return true;
            }
            int index = keyTime.IndexOf(';');
            long endTime = long.Parse(keyTime.Substring(index + 1));
            long nowTime = TimeUtils.GetCurrentTime(TimeUnit.SECONDS);
            if ((endTime - 10) <= nowTime) return true;
            return false;
        }
    }

}

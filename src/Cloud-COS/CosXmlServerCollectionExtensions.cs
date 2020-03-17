using COSXML;
using COSXML.Auth;
using COSXML.Common;
using COSXML.Utils;
using Hunter.Tencent.COSSDK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CosXmlServerCollectionExtensions
    {
        public static void AddCosXmlServer(this IServiceCollection services, IConfigurationRoot configuration, Action<CosXmlConfig.Builder> steup)
        {
            services.Configure(steup);
            services.Configure<CosXmlOptions>(options =>
            {
                configuration.GetSection("CosXmlOptions").Bind(options);
            });

            services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<CosXmlOptions>>()?.Value ?? throw new ArgumentNullException(nameof(CosXmlOptions));
                var config = sp.GetService<IOptions<CosXmlConfig.Builder>>()?.Value ?? throw new ArgumentNullException(nameof(CosXmlConfig.Builder));

                if (!string.IsNullOrEmpty(options.Appid))
                    config.SetAppid(options.Appid);

                if (!string.IsNullOrEmpty(options.Region))
                    config.SetRegion(options.Region);

                QCloudCredentialProvider cosCredentialProvider =new Hunter.Tencent.COSSDK.TencentQCloudCredentialProvider(options.QCloudCredential.TmpSecretId, options.QCloudCredential.TmpSecretKey, options.QCloudCredential.DurationSecond);
                return new CosXmlServer(
                config.Build(),
                cosCredentialProvider);
            });
        }

        public static void AddCosXmlServer(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<CosXmlOptions>(options =>
            {
                configuration.GetSection("CosXmlOptions").Bind(options);
            });
            services.AddSingleton<CosXmlServer>(sp =>
            {
                var options = sp.GetService<IOptions<CosXmlOptions>>()?.Value ?? throw new ArgumentNullException(nameof(CosXmlOptions));
                var config = new CosXmlConfig.Builder()
                    .SetAppid(options.Appid)
                    .SetRegion(options.Region);

                QCloudCredentialProvider cosCredentialProvider =new Hunter.Tencent.COSSDK.TencentQCloudCredentialProvider(options.QCloudCredential.TmpSecretId, options.QCloudCredential.TmpSecretKey, options.QCloudCredential.DurationSecond);
                return new CosXmlServer(
                config.Build(),
                cosCredentialProvider);
            });
        }
    }

  



}

# Cloud-COS
+ 添加对IServiceCollection 的扩展用于推送到腾讯云存储 AddCosXmlServer
+ 使用方法
Startup
```
        public Startup(IConfiguration configuration)
        {
            Configuration = (IConfigurationRoot) configuration;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCosXmlServer(configuration);
        }
```
appsettings.json
```
        "CosXmlOptions": {
            "QCloudCredential": {
              "TmpSecretId": "xxxxxxxxxxxxxxxxxxxxxxx",
              "TmpSecretKey": "xxxxxxxxxxxxxxx",
              "DurationSecond": "600"
            },
            "Region": "ap-guangzhou",
            "Appid": "xxxxxxxxxxxxxxxxxxxxxx",
            "Bucket": "xxxxxxxxxxx"
          }
```
```
        private readonly CosXmlServer _cosXmlServer;
        private readonly CosXmlOptions _cosXmlOptions;
        private readonly JsonSerializerSettings _serializerSetting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public LongTermJobCOSManager(ILogger<LongTermJobCOSManager> logger,
            CosXmlServer cosXmlServer,
            IOptions<CosXmlOptions> cosXmlOptions) 
        {
            _cosXmlServer = cosXmlServer;
            _cosXmlOptions = cosXmlOptions.Value;
            _logger = logger;
        }
          var jsonContent = JsonConvert.SerializeObject(longTermJobObjectModel, _serializerSetting);
          try
                {
                    //string bucket = "job-dev"; //存储桶，格式：BucketName-APPID
                    string key = $"/job/longterm/{longTermJobObjectModel.JobId}.json"; //对象在存储桶中的位置，即称对象键.
//string srcPath =           @"G:\c03fd54abe6e19a631bc59.jpg";//本地文件绝对路径
                    PutObjectRequest request = new PutObjectRequest(_cosXmlOptions.Bucket, key, Encoding.Default.GetBytes(jsonContent));
                    //设置签名有效时长
                    request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 60 * 60 * 5);
                    //执行请求
                    PutObjectResult result = _cosXmlServer.PutObject(request);

                }
                catch (COSXML.CosException.CosServerException serverEx)
                {
                    //请求失败
                    return HandleResult.Failed(ErrorBehavior.Retry, serverEx.GetInfo());
                }
```
![Image text](https://github.com/XZL-Github/images/blob/master/1.png)
![Image text](https://github.com/XZL-Github/images/blob/master/2.png)

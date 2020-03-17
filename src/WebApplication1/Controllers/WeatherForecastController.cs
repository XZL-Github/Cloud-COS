using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COSXML;
using COSXML.Model.Object;
using COSXML.Utils;
using Hunter.Tencent.COSSDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly CosXmlServer _cosXmlServer;
        private readonly CosXmlOptions _cosXmlOptions;
        private readonly JsonSerializerSettings _serializerSetting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public WeatherForecastController(ILogger<WeatherForecastController> logger,CosXmlServer cosXmlServer,
            IOptions<CosXmlOptions> cosXmlOptions)
        {
            _cosXmlServer = cosXmlServer;
            _cosXmlOptions = cosXmlOptions.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            LongTermJobObjectModel longTermJobObjectModel = new LongTermJobObjectModel() 
            { 
                Id=12,
                Name="xzlxzl",
                Age=20,
                Remark="测试一下"
            };

            var jsonContent = JsonConvert.SerializeObject(longTermJobObjectModel, _serializerSetting);
            try
            {
                //string bucket = "job-dev"; //存储桶，格式：BucketName-APPID
                string key = $"/test/{longTermJobObjectModel.Id}.json"; //对象在存储桶中的位置，即称对象键.
                                                                                   //string srcPath =           @"G:\c03fd54abe6e19a631bc59.jpg";//本地文件绝对路径
                PutObjectRequest request = new PutObjectRequest(_cosXmlOptions.Bucket, key, Encoding.Default.GetBytes(jsonContent));
                //设置签名有效时长
                request.SetSign(TimeUtils.GetCurrentTime(TimeUnit.SECONDS), 60 * 60 * 5);
                //执行请求
                PutObjectResult result = _cosXmlServer.PutObject(request);
                return Ok();
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                //请求失败
                Console.WriteLine(serverEx);
                return Ok(serverEx);
            }
            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();
        }
    }
    public class LongTermJobObjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Remark { get; set; }

    }
}

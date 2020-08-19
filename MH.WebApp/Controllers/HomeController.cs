using Crm.WebApp.AuthorizeHelper;
using Currency.Common.HttpHelperFile;
using Currency.Common.Redis;
using Currency.Weixin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MH.WebApp.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        public readonly BasicApi _basicApi;
        public readonly RedisManager _redisManager;
        public HomeController(BasicApi basicApi, RedisManager redisManager)
        {
            _basicApi = basicApi;
            redisManager.DbNum = 2;
            _redisManager = redisManager;
        }

        [HttpGet]
        [NoSign]
        public IActionResult Index()
        {
            return Json("hello word");
        }
        [HttpGet, NoSign]
        public async Task<string> GetDemo()
        {
            var res = await HttpHelper.HttpGetAsync("http://localhost:6657/home/getdemo1", HttpWebRequestContentType.ApplicationDefault);

            return res;
        }
        [HttpGet, NoSign]
        public IActionResult GetDemo1()
        {
            var model = new user
            {
                name = "陈浩",
                sex = 17,
                time = DateTime.Now
            };

            return Json(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet, NoSign]
        public IActionResult SaveRedisValue(string key, string value)
        {
            //_basicApi.GetAccessToken();
            var model = new user
            {
                name = "陈浩",
                sex = 17,
                time = DateTime.Now
            };
            _redisManager.DbNum = 1;
            var flag = _redisManager.StringSet(key, model);

            return Json("保存成功:" + flag);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet, NoSign]
        public IActionResult PullRedisValue(string key, string value)
        {
            var flag = _redisManager.StringGet<user>(key);

            return Json(flag);
        }

        public class user
        {
            public string name { get; set; }
            public int sex { get; set; }
            public DateTime time { get; set; }
        }
    }
}

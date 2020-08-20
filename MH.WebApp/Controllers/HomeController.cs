using Crm.WebApp.AuthorizeHelper;
using Currency.Common.HttpHelperFile;
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
        public HomeController()
        {
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

    }
}

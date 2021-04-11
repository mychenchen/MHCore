using Crm.WebApp.AuthorizeHelper;
using Currency.Common.HttpHelperFile;
using Currency.Models.Comm_Entity;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MH.WebApp.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        protected readonly WebAppSetting _webSetting;
        public HomeController(IOptions<WebAppSetting> webSetting)
        {
            _webSetting = webSetting.Value;
        }

        [HttpGet]
        [NoSign]
        public IActionResult Index()
        {
            return Json("hello word");
        }

        [HttpGet]
        public IActionResult Authenticate()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtClaimTypes.Audience,_webSetting.JwtAud),
                    new Claim(JwtClaimTypes.Issuer,_webSetting.JwtIss),
                    new Claim(JwtClaimTypes.Id, "1"),
                    new Claim(JwtClaimTypes.Name, "xxx"),
                    new Claim(JwtClaimTypes.Email, "xxx@qq.com"),
                    new Claim(JwtClaimTypes.PhoneNumber, "13500000000")
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(Startup.symmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    sid = "1",
                    name = "xxxx",
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }

        [Authorize]
        [HttpGet]
        public string Test2()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.Claims.FirstOrDefault(u => u.Type == JwtClaimTypes.Id).Value;
            return id;
        }
        [HttpGet]
        public async Task<string> GetDemo()
        {
            var res = await HttpHelper.HttpGetAsync("http://localhost:6657/home/getdemo1", HttpWebRequestContentType.ApplicationDefault);

            return res;
        }

    }
}

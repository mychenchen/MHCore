using Crm.WebApp.AuthorizeHelper;
using Currency.Weixin.WeChatComm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using System.Threading;

namespace MH.WebApp.Controllers
{
    /// <summary>
    /// 微信公众号配置
    /// </summary>
    public class WeixinController : Controller
    {
        protected readonly SenparcWeixinSetting _settWeiXin;
        //与微信公众账号后台的Token设置保持一致，区分大小写。
        protected readonly string Token;
        //与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        protected readonly string EncodingAESKey;
        //与微信公众账号后台的AppId设置保持一致，区分大小写。
        protected readonly string AppId;

        public WeixinController(IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            _settWeiXin = senparcWeixinSetting.Value;
            Token = _settWeiXin.Token;
            EncodingAESKey = _settWeiXin.EncodingAESKey;
            AppId = _settWeiXin.WeixinAppId;
        }

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://weixin.senparc.com/weixin
        /// </summary>
        [HttpGet, NoSign]
        public ActionResult Index(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost, NoSign]
        public ActionResult Index(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.Body, postModel);//接收消息
            messageHandler.ExecuteAsync(CancellationToken.None);
            return new FixWeixinBugWeixinResult(messageHandler);//返回结果
        }

    }
}

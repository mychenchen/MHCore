using Currency.Common.DIRegister;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.Containers;
using System;

namespace Currency.Weixin
{
    /// <summary>
    /// 微信操作API
    /// </summary>
    public class BasicApi
    {
        protected static SenparcWeixinSetting _senparcWeixinSetting;

        public BasicApi(IOptions<SenparcWeixinSetting> options)
        {
            _senparcWeixinSetting = options.Value;
        }

        #region 获取token

        /// <summary>
        /// 获取token(无缓存)
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken(string appid = "")
        {
            if (appid == "")
            {
                appid = _senparcWeixinSetting.WeixinAppId;
            }

            //根据appId判断获取  
            if (!AccessTokenContainer.CheckRegistered(_senparcWeixinSetting.WeixinAppId))    //检查是否已经注册  
            {
                //如果没有注册则进行注册  
                AccessTokenContainer.RegisterAsync(_senparcWeixinSetting.WeixinAppId, _senparcWeixinSetting.WeixinAppSecret);
            }

            return AccessTokenContainer.GetAccessToken(appid);
        }

        /// <summary>
        /// 获取token(带缓存)
        /// </summary>
        /// <returns></returns>
        public WeiXinToken GetAccessTokenCache(string appid = "")
        {
            //var model = RedisHelper<WeiXinToken>.GetValus("WeiXinToken");
            //if (model != null)
            //{
            //    return model;
            //}
            var model = new WeiXinToken();

            model.WxToken = GetAccessToken();
            model.ExpirationTime = DateTime.Now.AddMinutes(110);
            //微信token只有120分钟有效期,这里我们设置为110分钟,提前重新获取token
            //RedisHelper<string>.SetValus("WeiXinToken", JsonHelper.ObjectToJSON(model), model.ExpirationTime);
            return model;
        }

        /// <summary>
        /// 微信token
        /// </summary>
        public class WeiXinToken
        {
            /// <summary>
            /// 微信token
            /// </summary>
            public string WxToken { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public DateTime ExpirationTime { get; set; }
        }

        #endregion

        #region 用户授权

        /// <summary>
        /// 用户授权获取code
        /// </summary>
        /// <param name="redirectUrl">授权成功回调页面</param>
        /// <returns></returns>
        public string GetWxCode(string redirectUrl)
        {
            var wxCode = OAuthApi.GetAuthorizeUrl(_senparcWeixinSetting.WeixinAppId, redirectUrl, "", Senparc.Weixin.MP.OAuthScope.snsapi_userinfo);
            return wxCode;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="code">授权code</param>
        /// <returns></returns>
        public OAuthAccessTokenResult GetWxAuthorizationInfo(string code)
        {
            var userInfo = OAuthApi.GetAccessToken(_senparcWeixinSetting.WeixinAppId, _senparcWeixinSetting.WeixinAppSecret, code);
            return userInfo;
        }
        #endregion

        #region 微信消息模板,发送通知

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="userOpenId"></param>
        /// <param name="templateId"></param>
        /// <param name="url">//点击详情后跳转后的链接地址，为空则不跳转</param>
        /// <param name="templateData"></param>
        /// <returns></returns>
        public string SendTemplateMessage<T>(string userOpenId, string templateId, string url, T templateData)
        {
            string access_token = GetAccessToken();

            SendTemplateMessageResult sendResult = TemplateApi.SendTemplateMessage(access_token, userOpenId, templateId, url, templateData);

            //发送成功
            if (sendResult.errcode.ToString() == "请求成功")
            {
                return "发送成功";
            }
            else
            {
                return sendResult.errmsg;
            }

        }

        /// <summary>
        /// 发送模板消息-跳转至小程序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userOpenId"></param>
        /// <param name="templateId"></param>
        /// <param name="url"></param>
        /// <param name="templateData"></param>
        /// <param name="miniAppId"></param>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public string SendTemplateMessageToMiniApp<T>(string userOpenId, string templateId, string url, T templateData, string miniAppId, string pagePath)
        {
            string access_token = GetAccessToken();

            TemplateModel_MiniProgram miniModel = new TemplateModel_MiniProgram
            {
                appid = miniAppId,
                pagepath = pagePath
            };
            SendTemplateMessageResult sendResult = TemplateApi.SendTemplateMessage(access_token, userOpenId, templateId, url, templateData, miniModel);

            //发送成功
            if (sendResult.errcode.ToString() == "请求成功")
            {
                return "发送成功";
            }
            else
            {
                return sendResult.errmsg;
            }

        }

        #endregion

        #region 获取用户信息

        /// <summary>
        /// 微信公众号-获取当前用户个人信息
        /// </summary>
        /// <param name="access_token">基础支持的access_token(GetToken接口)</param>
        /// <param name="openid">用户唯一标识</param>
        /// <returns></returns>
        public UserInfoJson GetWxUserInfo(string access_token, string openid)
        {
            var info = UserApi.Info(access_token, openid);
            return info;
        }


        #endregion
    }
}

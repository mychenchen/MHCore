

using Currency.Common;
using Currency.Common.xml;
using Senparc.NeuChar;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System;
using System.Xml.Linq;

namespace Currency.Weixin.WeChatComm
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
    {
        public CustomMessageContext()
        {
            base.MessageContextRemoved += CustomMessageContext_MessageContextRemoved;
        }

        /// <summary>
        ///  从 Xml 转换 RequestMessage 对象的处理（只是创建实例，不填充数据）
        /// </summary>
        /// <param name="requestMsgType"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public override IRequestMessageBase GetRequestEntityMappingResult(RequestMsgType requestMsgType, XDocument doc)
        {
            var res = doc.ToString();
            IRequestMessageBase reqBase = new RequestMessageBase
            {
                ToUserName = XmlHelper.GetXmlNode(res, "ToUserName").Trim(),
                FromUserName = XmlHelper.GetXmlNode(res, "FromUserName").Trim(),
                Encrypt = XmlHelper.GetXmlNode(res, "Encrypt").Trim(),
                MsgId = Int64.Parse(XmlHelper.GetXmlNode(res, "MsgId")),
                MsgType = requestMsgType,
                CreateTime = TimeStampHelper.GetDateTime(int.Parse(XmlHelper.GetXmlNode(res, "CreateTime").Trim()))
            };

            //ApiLoghelper.Info("reqBase", JsonHelper.ObjectToJSON(reqBase));

            return reqBase;
        }


        /// <summary>
        ///  从 Xml 转换 RequestMessage 对象的处理（只是创建实例，不填充数据）
        /// </summary>
        /// <param name="responseMsgType"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public override IResponseMessageBase GetResponseEntityMappingResult(ResponseMsgType responseMsgType, XDocument doc)
        {
            var res = doc.ToString();
            IResponseMessageBase resBase = new ResponseMessageBase
            {

                ToUserName = XmlHelper.GetXmlNode(res, "ToUserName").Trim(),
                FromUserName = XmlHelper.GetXmlNode(res, "FromUserName").Trim(),
                MsgType = responseMsgType,
                CreateTime = TimeStampHelper.GetDateTime(int.Parse(XmlHelper.GetXmlNode(res, "CreateTime").Trim()))
            };
            return resBase;
        }


        /// <summary>
        /// 当上下文过期，被移除时触发的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CustomMessageContext_MessageContextRemoved(object sender, Senparc.NeuChar.Context.WeixinContextRemovedEventArgs<IRequestMessageBase, IResponseMessageBase> e)
        {
            /* 注意，这个事件不是实时触发的（当然你也可以专门写一个线程监控）
             * 为了提高效率，根据WeixinContext中的算法，这里的过期消息会在过期后下一条请求执行之前被清除
             */

            var messageContext = e.MessageContext as CustomMessageContext;
            if (messageContext == null)
            {
                return;//如果是正常的调用，messageContext不会为null
            }

            //TODO:这里根据需要执行消息过期时候的逻辑，下面的代码仅供参考

            //Log.InfoFormat("{0}的消息上下文已过期",e.OpenId);
            //api.SendMessage(e.OpenId, "由于长时间未搭理客服，您的客服状态已退出！");
        }

    }
}
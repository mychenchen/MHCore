using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System.IO;

namespace Currency.Weixin.WeChatComm
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel)
            : base(inputStream, postModel)
        {

        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            try
            {
                ResponseMessageText responseMessage = base.CreateResponseMessage<ResponseMessageText>(); //ResponseMessageText也可以是News等其他类型
                responseMessage.Content = "这条消息来自DefaultResponseMessage。";
                return responseMessage;
            }
            catch (System.Exception ex)
            {
                //ApiLoghelper.Info("DefaultResponseMessage", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 处理文字类请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            try
            {
                var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "您的OpenID是：" + requestMessage.FromUserName      //这里的requestMessage.FromUserName也可以直接写成base.WeixinOpenId
                                        + "。\r\n您发送了文字信息：" + requestMessage.Content;  //\r\n用于换行，requestMessage.Content即用户发过来的文字内容

                return responseMessage;
            }
            catch (System.Exception ex)
            {
                //ApiLoghelper.Info("OnTextRequest", ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 处理图片类请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            try
            {
                //ApiLoghelper.Info("OnTextRequest", "公众号内发送信息");
                var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "您的OpenID是：" + requestMessage.FromUserName      //这里的requestMessage.FromUserName也可以直接写成base.WeixinOpenId
                                        + "。\r\n您发送了图片链接：" + requestMessage.PicUrl;  //\r\n用于换行，requestMessage.Content即用户发过来的文字内容

                return responseMessage;
            }
            catch (System.Exception ex)
            {
                //ApiLoghelper.Info("OnTextRequest", ex.Message);
                throw;
            }
        }
        //public override void OnExecuting()
        //{
        //    //添加一条固定回复
        //    var responseMessage = CreateResponseMessage<ResponseMessageText>();
        //    responseMessage.Content = "Hey！你已经被拉黑啦！";

        //    ResponseMessage = responseMessage;//设置返回对象

        //}

    }

}
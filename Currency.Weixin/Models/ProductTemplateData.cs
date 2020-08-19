using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;

namespace Currency.Weixin.Models
{
    /* 
     此处为微信模板消息实体模型
     请保持模型与微信模板一致,否则消息将无法发送成功
     注:新模型请自行在此处添加
         */

    /// <summary>
    /// 微信消息模板(含 keyword1,keyword2)
    /// </summary>
    public class ProductTemplateData2 : ProducTemplateBase
    {
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
    }

    /// <summary>
    /// 微信消息模板(含 keyword1,keyword2,keyword3)
    /// </summary>
    public class ProductTemplateData3 : ProducTemplateBase
    {
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
    }

    /// <summary>
    ///  微信消息模板(含 keyword1,keyword2,keyword3,keyword4)
    /// </summary>
    public class ProductTemplateData4 : ProducTemplateBase
    {
        public TemplateDataItem keyword1 { get; set; }
        public TemplateDataItem keyword2 { get; set; }
        public TemplateDataItem keyword3 { get; set; }
        public TemplateDataItem keyword4 { get; set; }
    }


    public class ProducTemplateBase
    {
        /// <summary>
        /// * 最重要的文字
        /// </summary>
        public TemplateDataItem first { get; set; }

        /// <summary>
        /// * 备注的文字
        /// </summary>
        public TemplateDataItem remark { get; set; }
    }

}
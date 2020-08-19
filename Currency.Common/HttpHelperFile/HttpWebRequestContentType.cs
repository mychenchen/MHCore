namespace Currency.Common.HttpHelperFile
{
    /// <summary>
    /// Http Web Request ContentType 枚举
    /// </summary>
    public enum HttpWebRequestContentType
    {
        /// <summary>
        /// 默认的 application/x-www-form-urlencoded
        /// </summary>
        [Extension("默认的", "application/x-www-form-urlencoded")]
        ApplicationDefault,

        /// <summary>
        /// XHTML格式
        /// </summary>
        [Extension("XHTML格式", "application/xhtml+xml")]
        ApplicationXHTML,

        /// <summary>
        /// XML数据格式
        /// </summary>
        [Extension("XML数据格式", "application/xml")]
        ApplicationXML,

        /// <summary>
        /// Atom XML聚合格式
        /// </summary>
        [Extension("Atom XML聚合格式", "application/atom+xml")]
        ApplicationAtomXML,

        /// <summary>
        /// JSON数据格式
        /// </summary>
        [Extension("JSON数据格式", "application/json")]
        ApplicationJSON,

        /// <summary>
        /// pdf格式
        /// </summary>
        [Extension("pdf格式", "application/pdf")]
        ApplicationPDF,

        /// <summary>
        /// Word文档格式
        /// </summary>
        [Extension("Word文档格式", "application/msword")]
        ApplicationWord,

        /// <summary>
        /// 二进制流数据（如常见的文件下载）
        /// </summary>
        [Extension("二进制流数据（如常见的文件下载）", "application/octet-stream")]
        ApplicationStream,

        /// <summary>
        /// HTML格式
        /// </summary>
        [Extension("HTML格式", "text/html")]
        HTML,

        /// <summary>
        /// 纯文本格式
        /// </summary>
        [Extension("纯文本格式", "text/plain")]
        Plain,

        /// <summary>
        /// XML格式
        /// </summary>
        [Extension("XML格式", "text/xml")]
        Xml,

        /// <summary>
        /// 纯文本格式
        /// </summary>
        [Extension("纯文本格式", "image/gif")]
        GIF,

        /// <summary>
        /// 纯文本格式
        /// </summary>
        [Extension("纯文本格式", "image/jpeg")]
        Jpeg,

        /// <summary>
        /// png图片格式
        /// </summary>
        [Extension("png图片格式", "image/png")]
        Png,
    }
}

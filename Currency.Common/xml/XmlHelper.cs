using System.Xml;

namespace Currency.Common.xml
{
    public class XmlHelper
    {
        /// <summary>
        /// 获取XML指定节点的值
        /// </summary>
        /// <param name="xmlStr">对象</param>
        /// <param name="nodeStr">对象</param>
        /// <returns>返回节点值</returns>
        public static string GetXmlNode(string xmlStr, string nodeStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);
            string str = "";
            XmlNode root = doc.DocumentElement;
            foreach (XmlNode xmlNode in root.ChildNodes)
            {
                if (xmlNode.Name == nodeStr)
                {
                    str = xmlNode.InnerText;
                    break;
                }
                //foreach (XmlNode xmlElement in xmlNode.ChildNodes)
                //{
                //    str= xmlElement.InnerText;
                //}
            }
            return str;
        }

    }
}

using System;

namespace Currency.Common.HttpHelperFile
{
    /// <summary>
    /// 扩展属性
    /// </summary>
    public class ExtensionAttribute : Attribute
    {
        private string _description = "";

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        private string _extendValue;

        /// <summary>
        /// 
        /// </summary>
        public string ExtendValue
        {
            get { return _extendValue; }
            set { _extendValue = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ExtensionAttribute() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public ExtensionAttribute(string description)
        {
            _description = description;
        }

        // <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public ExtensionAttribute(string description, string extendValue)
        {
            _description = description;
            _extendValue = extendValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Currency.Models
{
    public class AutoMappersAttribute : Attribute
    {
        public Type[] ToSource { get; private set; }

        public AutoMappersAttribute(params Type[] toSource)
        {
            this.ToSource = toSource;
        }
    }
}

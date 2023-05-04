using System;

namespace SweetMeSoft.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TemplateAttribute : Attribute
    {
        public string Explanation { get; set; }

        public TemplateAttribute() { }
    }
}

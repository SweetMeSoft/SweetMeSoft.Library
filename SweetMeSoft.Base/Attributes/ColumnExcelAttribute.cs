using System;

namespace SweetMeSoft.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnExcelAttribute : Attribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }

        public ColumnExcelAttribute()
        {
        }

        public ColumnExcelAttribute(string name)
        {
            Name = name;
            Format = "yyyy-MM-dd";
        }
    }
}

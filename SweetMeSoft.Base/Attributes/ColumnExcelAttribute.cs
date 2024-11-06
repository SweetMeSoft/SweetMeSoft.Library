using SweetMeSoft.Base.Files;

namespace SweetMeSoft.Base.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnExcelAttribute : Attribute
{
    public string Name { get; set; }

    public ExcelColumnType Type { get; set; }

    public string DateFormat { get; set; }

    public string BoolTrueValue { get; set; }

    public string BoolFalseValue { get; set; }

    public ColumnExcelAttribute()
    {
    }

    public ColumnExcelAttribute(string name)
    {
        Name = name;
        DateFormat = "yyyy-MM-dd";
        BoolTrueValue = "1";
        BoolFalseValue = "0";
        Type = ExcelColumnType.String;
    }
}
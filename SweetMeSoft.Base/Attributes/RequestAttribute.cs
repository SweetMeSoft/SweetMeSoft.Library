namespace SweetMeSoft.Base.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class RequestAttribute : Attribute
{
    public string Name { get; set; }

    public string Type { get; set; }

    public RequestAttribute()
    {
    }

    public RequestAttribute(string name)
    {
        Name = name;
    }
}
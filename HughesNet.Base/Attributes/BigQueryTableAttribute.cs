using System.Runtime.CompilerServices;

namespace HughesNet.Base.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BigQueryTableAttribute : Attribute
{
    public string Name { get; set; }

    public string Dataset { get; set; }

    public BigQueryTableAttribute([CallerMemberName] string className = null)
    {
        Name = className;
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SweetMeSoft.Tools;

public class Reflections
{
    public static T CleanVirtualProperties<T>(T entity)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var s = JsonSerializer.Serialize(entity, options);
        return JsonSerializer.Deserialize<T>(s, options);
    }
}
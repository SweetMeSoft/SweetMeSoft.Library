using Microsoft.UI.Xaml.Markup;

using System.Globalization;
using System.Reflection;
using System.Resources;

namespace SweetMeSoft.Uno.Base.i18n;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public class TranslateExtension : MarkupExtension
{
    private readonly CultureInfo ci;
    private readonly Lazy<ResourceManager> InternalManager = new(() => new ResourceManager("SweetMeSoft.Uno.Base.i18n.Resources", typeof(Resources.Resources).GetTypeInfo().Assembly));
    private readonly Lazy<ResourceManager> ExternalManager = new(() => new ResourceManager(UnoConstants.ExternalId, UnoConstants.TypeResources?.GetTypeInfo().Assembly));

    public TranslateExtension()
    {
        ci = CultureInfo.CurrentCulture;
    }

    public string Text { get; set; }

    protected override object ProvideValue()
    {
        if (Text == null)
        {
            return "";
        }

        var value = ExternalManager.Value;
        var s = value.GetString(Text, ci);
        if (s == null)
        {
            value = InternalManager.Value;
            s = value.GetString(Text, ci);
        }

        var translation = s ?? "Key '" + Text + "' was not found for culture '" + ci.Name + "'.";
        return translation;
    }
}

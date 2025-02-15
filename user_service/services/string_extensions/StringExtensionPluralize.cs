namespace user_service.services.string_extensions;

public static class StringExtensionPluralize
{
    public static string Pluralize(this string text)
    {
        if(text == null) {
            throw new ArgumentNullException(nameof(text));
        }
        if(text.EndsWith("y"))
            text = text.Substring(0, text.Length - 1) + "ies";
        else
            text += "s";
            
        return text;
    }
}

using System.Reflection;

namespace Support.Auth.Id.Features.Helper;

public static class HtmlTemplateEngine
{
    public static string Render(string templateFileName, Dictionary<string, string> replacements)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(templateFileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName == null)
            throw new FileNotFoundException($"Template '{templateFileName}' is not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        foreach (var (key, value) in replacements)
        {
            content = content.Replace($"{{{{{key}}}}}", value);
        }

        return content;
    }
}
using Microsoft.Extensions.Configuration;

internal static class ResourceBuilderConfigurationExtensions
{
    public static IResourceBuilder<T> WithEnvironmentSection<T>(
        this IResourceBuilder<T> builder,
        IConfiguration configuration,
        string sectionPath)
        where T : IResourceWithEnvironment
    {
        var section = configuration.GetRequiredSection(sectionPath);

        foreach (var (key, value) in EnumerateSectionValues(section))
            builder.WithEnvironment(key.Replace(":", "__"), value);

        return builder;
    }

    private static IEnumerable<KeyValuePair<string, string>> EnumerateSectionValues(
        IConfigurationSection section,
        string? prefix = null)
    {
        foreach (var child in section.GetChildren())
        {
            var key = string.IsNullOrWhiteSpace(prefix)
                ? child.Key
                : $"{prefix}:{child.Key}";

            if (child.Value is not null)
                yield return new KeyValuePair<string, string>(key, child.Value);

            foreach (var item in EnumerateSectionValues(child, key))
                yield return item;
        }
    }
}
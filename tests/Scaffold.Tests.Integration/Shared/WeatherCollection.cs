using BuildingBlocks.Tests.Integration;
using Xunit;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// xUnit collection definition shared by all Weather integration tests.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class WeatherCollection : IntegrationTestCollection<WeatherEnvironment>
{
    public const string Name = "Weather";
}

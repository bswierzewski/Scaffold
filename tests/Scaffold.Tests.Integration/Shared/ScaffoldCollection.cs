using BuildingBlocks.Tests.Integration;
using Xunit;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// xUnit collection definition shared by all integration tests.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ScaffoldCollection : IntegrationTestCollection<ScaffoldEnvironment>
{
    public const string Name = "Scaffold";
}
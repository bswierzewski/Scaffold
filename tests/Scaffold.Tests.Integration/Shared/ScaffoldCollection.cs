using BuildingBlocks.Tests.Integration.Fixtures;

namespace Scaffold.Tests.Integration.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ScaffoldCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Scaffold";
}
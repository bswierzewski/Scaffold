namespace Scaffold.Tests.E2E.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ScaffoldCollection : ICollectionFixture<ScaffoldEnvironment>
{
    public const string Name = "Scaffold";
}
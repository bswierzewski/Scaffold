namespace Scaffold.Tests.Integration.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ScaffoldCollection : ICollectionFixture<ScaffoldEnvironment>
{
    public const string Name = "Scaffold";
}
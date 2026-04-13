using BuildingBlocks.Tests.E2E;
using Xunit;

namespace Scaffold.Tests.E2E.Shared;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ScaffoldCollection : EndToEndTestCollection<ScaffoldEnvironment>
{
    public const string Name = "Scaffold";
}
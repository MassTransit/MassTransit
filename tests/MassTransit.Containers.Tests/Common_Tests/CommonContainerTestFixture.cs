namespace MassTransit.Containers.Tests.Common_Tests
{
    using NUnit.Framework;
    using TestFramework;


    [TestFixture(typeof(DependencyInjectionTestFixtureContainerFactory))]
    public class CommonContainerTestFixture<TContainer> :
        InMemoryContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
    }
}

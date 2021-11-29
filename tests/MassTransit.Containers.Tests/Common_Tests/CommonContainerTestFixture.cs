namespace MassTransit.Containers.Tests.Common_Tests
{
    using NUnit.Framework;
    using TestFramework;


    [TestFixture(typeof(AutofacTestFixtureContainerFactory))]
    [TestFixture(typeof(DependencyInjectionTestFixtureContainerFactory))]
    [TestFixture(typeof(LamarTestFixtureContainerFactory))]
    [TestFixture(typeof(SimpleInjectorTestFixtureContainerFactory))]
    [TestFixture(typeof(StructureMapTestFixtureContainerFactory))]
    [TestFixture(typeof(CastleWindsorTestFixtureContainerFactory))]
    public class CommonContainerTestFixture<TContainer> :
        InMemoryContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
    }
}

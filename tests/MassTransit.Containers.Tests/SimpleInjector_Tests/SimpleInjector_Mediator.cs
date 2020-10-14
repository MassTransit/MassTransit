namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using Mediator;
    using NUnit.Framework;
    using SimpleInjector;


    [TestFixture]
    public class SimpleInjector_Mediator :
        Common_Mediator
    {
        readonly Container _container;

        public SimpleInjector_Mediator()
        {
            _container = new Container();
            _container.SetRequiredOptions();
            _container.AddMediator(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IMediator Mediator => _container.GetInstance<IMediator>();
    }
}

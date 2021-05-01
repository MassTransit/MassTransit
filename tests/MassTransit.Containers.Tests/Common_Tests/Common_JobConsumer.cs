namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Definition;
    using JobConsumerComponents;
    using JobConsumerContracts;
    using JobService.Configuration;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public abstract class Common_JobConsumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_resolve_an_use_the_service()
        {
            IRequestClient<CrunchTheNumbers> client = RequestClient;

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new {Duration = TimeSpan.FromSeconds(1)});

            Assert.That(response.SourceAddress, Is.EqualTo(new Uri(BaseAddress, KebabCaseEndpointNameFormatter.Instance.Consumer<CrunchTheNumbersConsumer>())));
        }

        protected Common_JobConsumer()
        {
            Options = new ServiceInstanceOptions()
                .EnableJobServiceEndpoints();
        }

        protected ServiceInstanceOptions Options { get; private set; }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            ConfigureServiceEndpoints(configurator);
        }

        protected abstract void ConfigureServiceEndpoints(IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator> configurator);

        protected abstract IRequestClient<CrunchTheNumbers> RequestClient { get; }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumersFromNamespaceContaining<CrunchTheNumbersConsumer>();

            configurator.AddRequestClient<CrunchTheNumbers>();

            configurator.AddBus(provider => BusControl);
        }
    }


    namespace JobConsumerContracts
    {
        using System;


        public interface CrunchTheNumbers
        {
            TimeSpan Duration { get; }
        }
    }


    namespace JobConsumerComponents
    {
        using JobService;


        public class CrunchTheNumbersConsumer :
            IJobConsumer<CrunchTheNumbers>
        {
            public async Task Run(JobContext<CrunchTheNumbers> context)
            {
                await Task.Delay(context.Job.Duration);
            }
        }
    }
}

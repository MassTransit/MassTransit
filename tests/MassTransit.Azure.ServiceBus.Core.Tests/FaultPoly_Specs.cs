namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core.Tests;
    using FaultMessages;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Explicit]
    public class Publishing_a_fault_message :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_support_the_base_fault_type()
        {
            Task<ConsumeContext<Fault<MemberUpdateCommand>>> handler = await ConnectPublishHandler<Fault<MemberUpdateCommand>>();

            await InputQueueSendEndpoint.Send<UpdateMemberAddress>(new
            {
                MemberName = "Frank",
                Address = "123 American Way"
            });

            await handler;
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<UpdateMemberAddress>(async context => throw new IntentionalTestException());
        }
    }


    namespace FaultMessages
    {
        [ExcludeFromTopology]
        [ExcludeFromImplementedTypes]
        public interface ICommand
        {
        }


        public interface MemberUpdateCommand :
            ICommand
        {
            string MemberName { get; }
        }


        public interface UpdateMemberAddress :
            MemberUpdateCommand
        {
            string Address { get; }
        }


        public class MemberUpdateEvent
        {
            public string MemberName { get; set; }
        }


        public class MemberAddressUpdated :
            MemberUpdateEvent
        {
            public string Address { get; set; }
        }
    }
}

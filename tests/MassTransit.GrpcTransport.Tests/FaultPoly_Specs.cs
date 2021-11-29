namespace MassTransit.GrpcTransport.Tests
{
    using System.Threading.Tasks;
    using FaultMessages;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Publishing_a_fault_message :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_support_the_base_fault_type()
        {
            await InputQueueSendEndpoint.Send<UpdateMemberAddress>(new
            {
                MemberName = "Frank",
                Address = "123 American Way"
            });

            await _handled;
        }

        Task<ConsumeContext<Fault<MemberUpdateCommand>>> _handled;

        protected override void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(e =>
            {
                _handled = Handled<Fault<MemberUpdateCommand>>(e);
            });
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<UpdateMemberAddress>(async context => throw new IntentionalTestException());
        }
    }


    namespace FaultMessages
    {
        public interface MemberUpdateCommand
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

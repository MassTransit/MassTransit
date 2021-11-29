namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using FaultMessages;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class A_fault_message
    {
        [Test]
        public void Should_have_the_fault_base_message_class_type()
        {
            Assert.That(MessageTypeCache<Fault<MemberAddressUpdated>>.MessageTypeNames, Contains.Item(MessageUrn.ForType(typeof(Fault<MemberUpdateEvent>))));
        }

        [Test]
        public void Should_have_the_fault_base_message_type()
        {
            Assert.That(MessageTypeCache<Fault<UpdateMemberAddress>>.MessageTypeNames, Contains.Item(MessageUrn.ForType(typeof(Fault<MemberUpdateCommand>))));
        }

        [Test]
        public void Should_have_the_fault_message_class_type()
        {
            Assert.That(MessageTypeCache<Fault<MemberAddressUpdated>>.MessageTypeNames,
                Contains.Item(MessageUrn.ForType(typeof(Fault<MemberAddressUpdated>))));
        }

        [Test]
        public void Should_have_the_fault_message_type()
        {
            Assert.That(MessageTypeCache<Fault<UpdateMemberAddress>>.MessageTypeNames, Contains.Item(MessageUrn.ForType(typeof(Fault<UpdateMemberAddress>))));
        }
    }


    [TestFixture]
    public class Publishing_a_fault_message :
        InMemoryTestFixture
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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
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

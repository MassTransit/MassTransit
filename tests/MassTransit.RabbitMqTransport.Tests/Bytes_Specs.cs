namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Bytes_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_byte_array_of_bigness()
        {
            var random = new Random();
            var bytes = new byte[512];
            for (var i = 0; i < 512; i++)
                bytes[i] = (byte)random.Next(255);

            var sent = new A { Contents = bytes };
            await InputQueueSendEndpoint.Send(sent);

            ConsumeContext<A> context = await _received;

            context.Message.ShouldBe(sent);
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = Handled<A>(configurator);
        }


        class A
        {
            public byte[] Contents { get; set; }

            public bool Equals(A other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return other.Contents.SequenceEqual(Contents);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(A))
                    return false;
                return Equals((A)obj);
            }

            public override int GetHashCode()
            {
                return Contents != null ? Contents.GetHashCode() : 0;
            }
        }
    }
}

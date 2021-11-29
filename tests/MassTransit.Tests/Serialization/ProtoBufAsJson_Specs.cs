namespace MassTransit.Tests.Serialization
{
    using System;
    using MassTransit.Serialization;
    using NUnit.Framework;


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    public class Serializing_a_protocol_buffer_message :
        SerializationTest
    {
        [Test]
        public void Should_return_the_array_values()
        {
            var tb = new TradesBookedMT();
            tb.Trades.Add(new TradeBookedMT {Currency = "AUD"});
            tb.Trades.Add(new TradeBookedMT {Currency = "USD"});

            var result = SerializeAndReturn(tb);

            Assert.That(result.Trades, Is.Not.Null);
            Assert.That(result.Trades.Count, Is.EqualTo(2));
        }

        public Serializing_a_protocol_buffer_message(Type serializerType)
            : base(serializerType)
        {
        }
    }
}

namespace MassTransit.ServiceBus.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class WriteOnlyMessageQueueEndpointTests :
        ServiceBusSetupFixture
    {
        private WriteOnlyMessageQueueEndpoint endpoint;

        public override void Before_Each_Test_In_The_Fixture()
        {
            base.Before_Each_Test_In_The_Fixture();
            endpoint = new WriteOnlyMessageQueueEndpoint(@".\private$\test_poison");
        }

        public override void After_Each_Test_In_The_Fixture()
        {
            base.After_Each_Test_In_The_Fixture();
            endpoint = null;
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void Poison_Not_Implemented()
        {
            object o = endpoint.Poison;
        }

        [Test]
        public void Address_Is_Right()
        {
            Assert.IsTrue(endpoint.Address.Contains("test_poison"));
        }

        [Test]
        public void Writes_Correctly()
        {
            DeleteMessage msg = new DeleteMessage();
            Envelope env = new Envelope(base._testEndPoint, msg);
            endpoint.Send(env);
            VerifyMessageInQueue(endpoint.Address, msg);
        }
    }
}
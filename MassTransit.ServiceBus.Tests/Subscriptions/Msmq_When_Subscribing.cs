//namespace PhatBoyG.ServiceBusTests.Subscriptions
//{
//    using System.Collections;
//    using NUnit.Framework;
//    using NUnit.Framework.SyntaxHelpers;
//    using PhatBoyG.ServiceBus.Subscriptions;
//    using Rhino.Mocks;
//    using ServiceBus;

//    [TestFixture]
//    public class Msmq_When_Subscribing 
//        : ServiceBusSetupFixture
//    {
//        private MockRepository mocks;
//        private MsmqSubscriptionManager mgr;
//        private IEndpoint mockEndpoint;
//        private string subscriptionQueueName = @".\private$\test_subscriptions";

//        [SetUp]
//        public void SetUp()
//        {
//            mocks = new MockRepository();
//            ValidateAndPurgeQueue(subscriptionQueueName);
//            mgr = new MsmqSubscriptionManager(subscriptionQueueName);
//            mockEndpoint = mocks.CreateMock<IEndpoint>();
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            mocks = null;
//            mgr = null;
//            mockEndpoint = null;
//            TeardownQueue(subscriptionQueueName);
//        }

//        [Test]
//        public void When_Adding_A_Message_It_Should_Be_Kept()
//        {
//            SubscriptionMessage msg = new SubscriptionMessage("a","a");
//            Envelope env = new Envelope(_serviceBusEndPoint, msg);
//            mgr.Add(env);

//            Assert.That(mgr.GetAllMessages().Count, Is.EqualTo(1));
//        }

//        [Test]
//        public void When_Adding_Multiple_Message_They_Should_Be_Kept()
//        {
//            SubscriptionMessage msg = new SubscriptionMessage("a", "a");
//            Envelope env = new Envelope(mockEndpoint, msg);
            
//            mgr.Add(env);

//            SubscriptionMessage msg2 = new SubscriptionMessage("b", "b");
//            Envelope env2 = new Envelope(mockEndpoint, msg2);
//            env2.Id = "dru"; //TODO: Ugly code
//            mgr.Add(env2);

//            Assert.That(mgr.GetAllMessages().Count, Is.EqualTo(2));
//        }

//        [Test]
//        public void Removing_Messages_They_Should_Be_Removed()
//        {
//            SubscriptionMessage msg = new SubscriptionMessage("a", "a");
//            Envelope env = new Envelope(mockEndpoint, msg);
//            mgr.Add(env);

//            Assert.That(mgr.GetAllMessages().Count, Is.EqualTo(1));

//            mgr.Remove(env);

//            Assert.That(mgr.GetAllMessages().Count, Is.EqualTo(0));
//        }

//        [Test]
//        public void When_Adding_Messsages_They_Should_Be_Idempotent()
//        {
//            SubscriptionMessage msg = new SubscriptionMessage("a", "a");
//            Envelope env = new Envelope(mockEndpoint, msg);
//            mgr.Add(env);
//            mgr.Add(env);

//            Assert.That(mgr.GetAllMessages().Count, Is.EqualTo(1));
//        }

//        [Test]
//        public void When_Removing_Messsages_They_Should_Be_Idempotent()
//        {
//            SubscriptionMessage msg = new SubscriptionMessage("a", "a");
//            Envelope env = new Envelope(mockEndpoint, msg);
//            mgr.Add(env);

//            mgr.Remove(env);
//            mgr.Remove(env);
//            Assert.That(mgr.GetAllMessages().Count, Is.EqualTo(0));
//        }

//        [Test]
//        public void If_You_Add_Some_Stuff_You_SHould_Get_It_Back()
//        {
//            SubscriptionMessage msg = new SubscriptionMessage("a", "a");
//            Envelope env = new Envelope(mockEndpoint, msg);
//            mgr.Add(env);

//            SubscriptionMessage msg2 = new SubscriptionMessage("b", "b");
//            Envelope env2 = new Envelope(mockEndpoint, msg2);
//            mgr.Add(env2);
            
//            Assert.Contains(env, (ICollection) mgr.GetAllMessages());
//            Assert.Contains(env2, (ICollection) mgr.GetAllMessages());
//        }
//    }
//}

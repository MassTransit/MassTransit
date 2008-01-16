using System;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
	[TestFixture]
	public class When_Publishing_A_Message
	{
	    private IServiceBus _serviceBus;
	    private IMessageQueueEndpoint _busEndpoint;
	    private ISubscriptionStorage _subs;
	    private MockRepository mocks;

	    private string queueName = @".\private$\test";
	    private Uri queueUri = new Uri("msmq://" + Environment.MachineName + "/test");




	    [SetUp]
	    public void SetUp()
	    {
            mocks = new MockRepository();
	        _busEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
	        _subs = mocks.CreateMock<ISubscriptionStorage>();
	        ServiceBusSetupFixture.ValidateAndPurgeQueue(queueName);
	    }

	    [TearDown]
	    public void TearDown()
	    {
	        mocks = null;
	        _busEndpoint = null;
	        _subs = null;
	        _serviceBus = null;
	    }

        [Test]
        [Ignore("working on it")]
        public void Poison_Letters_Should_Be_Moved_To_A_Poison_Queue()
        {
            using(mocks.Record())
            {
                Expect.Call(_busEndpoint.QueueName).Return(queueName);
                Expect.Call(_busEndpoint.Uri).Return(queueUri);
                Expect.Call(_busEndpoint.QueueName).Return(queueName);
                //Expect.Call(_busEndpoint.Uri).Return(queueUri);
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(_busEndpoint, _subs);

                //ManualResetEvent updateEvent = new ManualResetEvent(false);
                //MessageQueueEndpoint poisonEndpoint = new MessageQueueEndpoint(_serviceBus.PoisonEndpoint.Uri);

                //this ends up in a seperate thread and I am therefore unable to figure out how to test
                //_serviceBus.Subscribe<PoisonMessage>(delegate(MessageContext<PoisonMessage> cxt)
                //                                         {
                //                                             try
                //                                             {
                //                                                 cxt.Message.ThrowException();
                //                                             }
                //                                             catch (Exception)
                //                                             {
                //                                                 cxt.MarkPoison();
                //                                             }
                //                                         }
                //    );

                //_serviceBus.Publish(new PoisonMessage());

                //updateEvent.WaitOne(TimeSpan.FromSeconds(3), true);
                //ServiceBusSetupFixture.VerifyMessageInQueue(poisonEndpoint, new PoisonMessage());
            }
        }
	}
}
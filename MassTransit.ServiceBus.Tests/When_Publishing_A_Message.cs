using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace MassTransit.ServiceBus.Tests
{
	[TestFixture]
	public class When_Publishing_A_Message
	{
	    private ServiceBus _serviceBus;
        private MockRepository mocks;
	    private IMessageQueueEndpoint mockBusEndpoint;
	    private ISubscriptionStorage mockSubscriptionStorage;
	    private IMessageSenderFactory mockSenderFactory;
	    private IMessageSender mockSender;
	    private IMessageReceiverFactory mockReceiverFactory;
	    private IMessageReceiver mockReceiver;

	    private string queueName = @".\private$\test";
	    private Uri queueUri = new Uri("msmq://" + Environment.MachineName + "/test");


	    [SetUp]
	    public void SetUp()
	    {
            mocks = new MockRepository();
	        mockBusEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
	        mockSubscriptionStorage = mocks.CreateMock<ISubscriptionStorage>();
	        ServiceBusSetupFixture.ValidateAndPurgeQueue(queueName);
	        mockSenderFactory = mocks.CreateMock<IMessageSenderFactory>();
	        mockSender = mocks.CreateMock<IMessageSender>();
	        mockReceiverFactory = mocks.CreateMock<IMessageReceiverFactory>();
	        mockReceiver = mocks.CreateMock<IMessageReceiver>();
	    }

	    [TearDown]
	    public void TearDown()
	    {
	        mocks = null;
	        mockBusEndpoint = null;
	        mockSubscriptionStorage = null;
	        _serviceBus = null;
	        mockSenderFactory = null;
	        mockReceiverFactory = null;
	        mockReceiver = null;
	    }

        [Test]
        public void Poison_Letters_Should_Be_Moved_To_A_Poison_Queue()
        {
            using(mocks.Record())
            {
                Expect.Call(mockReceiverFactory.Using(mockBusEndpoint)).Return(mockReceiver);
                mockReceiver.Subscribe(_serviceBus); //TODO: Actions in SB contstructor causing this issue
                LastCall.IgnoreArguments();
                Expect.Call(mockSenderFactory.Using(mockBusEndpoint)).Return(mockSender);
                Expect.Call(mockBusEndpoint.Uri).Return(queueUri);
                mockSubscriptionStorage.Add(typeof(PoisonMessage).FullName, this.queueUri);

                Expect.Call(mockBusEndpoint.Uri).Return(queueUri);
                Expect.Call(mockSenderFactory.Using(new MessageQueueEndpoint(queueUri))).Return(mockSender).IgnoreArguments(); //Ignoring arguments because we should be using the poison endpoint

                mockSender.Send(null);
                LastCall.IgnoreArguments(); //because we can't control the Envelope from here?
            }
            using (mocks.Playback())
            {
                _serviceBus = new ServiceBus(mockBusEndpoint, mockSubscriptionStorage, mockSenderFactory, mockReceiverFactory);
                
                ////this ends up in a seperate thread and I am therefore unable to figure out how to test
                _serviceBus.Subscribe<PoisonMessage>(delegate(IMessageContext<PoisonMessage> cxt)
                                                         {
                                                             try
                                                             {
                                                                 cxt.Message.ThrowException();
                                                                 Assert.Fail("No Exception was thrown");
                                                             }
                                                             catch (Exception)
                                                             {
                                                                 cxt.MarkPoison();
                                                             }
                                                         }
                    );

                _serviceBus.Deliver(new Envelope(new PoisonMessage()));
            }
        }
	}
}
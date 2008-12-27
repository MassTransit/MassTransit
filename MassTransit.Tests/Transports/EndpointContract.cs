namespace MassTransit.Tests.Transports
{
    using System;
    using System.Transactions;
    using Configuration;
    using Magnum.Common.DateTimeExtensions;
    using NUnit.Framework;

    public abstract class EndpointContract<TEndpoint> where TEndpoint : IEndpoint
    {
        private IEndpoint _ep;
        private IEndpointFactory _endpointFactory;
        public IObjectBuilder ObjectBuilder { get; set; }
        public Uri Address { get; set; }
        public Action<Uri> VerifyMessageIsInQueue { get; set; }
        public Action<Uri> VerifyMessageIsNotInQueue { get; set; }

        [SetUp]
        public void SetUp()
        {
            _endpointFactory = EndpointFactoryConfigurator.New(c =>
            {
                c.RegisterTransport<TEndpoint>();
                c.SetObjectBuilder(ObjectBuilder);
            });
            _ep = _endpointFactory.GetEndpoint(Address);
        }

        [TearDown]
        public void TearDown()
        {
            _ep.Dispose();
            _ep = null;
        }


        [NUnit.Framework.Test]
        public void While_writing_it_should_perisist_on_complete()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                trx.Complete();
            }

            VerifyMessageIsInQueue(Address);
        }

        [NUnit.Framework.Test]
        public void While_writing_it_should_perisist_even_on_rollback()
        {
            using (TransactionScope trx = new TransactionScope())
            {
                _ep.Send(new DeleteMessage());
                //no complete
            }

            VerifyMessageIsInQueue(Address);
        }

        //outside transaction
        [Test]
        public void While_writing_it_should_persist()
        {
            _ep.Send(new DeleteMessage());

            VerifyMessageIsInQueue(Address);
        }

        [Test]
        public void While_reading_it_should_pull_object_from_queue()
        {
            object obj;
            _ep.Send(new DeleteMessage());
            
            //todo how to get the message back out
            _ep.Receive(5.Seconds(), (message, receiver)=>
                                                               {
                                                                   obj = message;
                                                                   return receiver(message);
                                                               });

            //obj.ShouldNotBeNull();
            //obj.ShouldBeSameType<DeleteMessage>();
        }
    }
}
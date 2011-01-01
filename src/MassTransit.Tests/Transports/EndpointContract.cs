namespace MassTransit.Tests.Transports
{
    using System;
    using System.Transactions;
    using Configuration;
    using NUnit.Framework;

    public abstract class EndpointContract<TEndpoint> where TEndpoint : IEndpoint
    {
        private IEndpoint _ep;
        private IEndpointResolver _endpointResolver;
        public IObjectBuilder ObjectBuilder { get; set; }
        public Uri Address { get; set; }
        public Action<Uri> VerifyMessageIsInQueue { get; set; }
        public Action<Uri> VerifyMessageIsNotInQueue { get; set; }

        [SetUp]
        public void SetUp()
        {
            _endpointResolver = EndpointResolverConfigurator.New(c =>
            {
                c.RegisterTransport<TEndpoint>();
                c.SetObjectBuilder(ObjectBuilder);
            });
            _ep = _endpointResolver.GetEndpoint(Address);
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
    }
}
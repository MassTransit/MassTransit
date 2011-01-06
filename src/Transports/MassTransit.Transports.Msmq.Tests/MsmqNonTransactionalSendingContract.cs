namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using NUnit.Framework;
    using TestFramework.Transports;

    [TestFixture,Category("Integration")]
    public class MsmqNonTransactionalSendingContract :
        NonTransactionalTransportSendingContract<MsmqTransportFactory>
    {
        public MsmqNonTransactionalSendingContract():base(new Uri("msmq://localhost/mt_client"), new MsmqTransportFactory())
        {
        }
    }
}
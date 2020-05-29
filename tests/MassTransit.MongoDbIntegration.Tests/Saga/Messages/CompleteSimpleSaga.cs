namespace MassTransit.MongoDbIntegration.Tests.Saga
{
    using System;


    public class CompleteSimpleSaga :
        SimpleSagaMessageBase
    {
        public CompleteSimpleSaga(Guid correlationId)
            : base(correlationId)
        {
        }
    }
}

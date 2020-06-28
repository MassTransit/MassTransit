namespace MassTransit.MongoDbIntegration.Saga
{
    using System;
    using MassTransit.Saga;


    [Obsolete("Use ISagaVersion (from the MassTransit.Saga namespace) instead")]
    public interface IVersionedSaga :
        ISagaVersion
    {
    }
}

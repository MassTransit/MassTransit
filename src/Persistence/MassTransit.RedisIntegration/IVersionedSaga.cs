namespace MassTransit.RedisIntegration
{
    using System;
    using Saga;


    [Obsolete("Use ISagaVersion (from the MassTransit.Saga namespace) instead")]
    public interface IVersionedSaga :
        ISagaVersion
    {
    }
}

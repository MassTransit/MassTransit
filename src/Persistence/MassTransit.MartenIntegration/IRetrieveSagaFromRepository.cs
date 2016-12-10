namespace MassTransit.MartenIntegration
{
    using System;
    using Saga;


    public interface IRetrieveSagaFromRepository<out TSaga>
        where TSaga : ISaga
    {
        TSaga GetSaga(Guid correlationId);
    }
}
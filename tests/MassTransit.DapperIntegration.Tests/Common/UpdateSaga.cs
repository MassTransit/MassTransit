namespace MassTransit.DapperIntegration.Tests.Common
{
    using System;


    public interface UpdateSaga : CorrelatedBy<Guid> { string Name { get; } }
}

namespace MassTransit.Persistence.Tests.Common
{
    public interface CreateSaga : CorrelatedBy<Guid>
    {
        string Name { get; }
    }
}

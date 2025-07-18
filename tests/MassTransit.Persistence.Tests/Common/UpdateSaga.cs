namespace MassTransit.Persistence.Tests.Common
{
    public interface UpdateSaga : CorrelatedBy<Guid>
    {
        string Name { get; }
    }
}

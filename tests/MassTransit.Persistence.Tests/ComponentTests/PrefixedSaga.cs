namespace MassTransit.Persistence.Tests.ComponentTests
{
    public class PrefixedSaga : ISaga
    {
        public int Id { get; set; }
        public NestedDependency Nested { get; set; }
        public OptionalDependency Optional { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public interface NestedDependency
    {
        int Id { get; }
        string Name { get; }
    }


    public interface OptionalDependency
    {
        int Id { get; }
        string Color { get; }
    }
}

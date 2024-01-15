namespace MassTransit.Tests.ContainerTests.Scenarios
{
    using System.Threading.Tasks;


    public interface ISimpleConsumerDependency
    {
        Task<bool> WasDisposed { get; }
        bool SomethingDone { get; }
        void DoSomething();
    }
}

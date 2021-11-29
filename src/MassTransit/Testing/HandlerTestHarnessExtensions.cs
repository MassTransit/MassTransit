namespace MassTransit.Testing
{
    using System.Threading.Tasks;


    public static class HandlerTestHarnessExtensions
    {
        public static HandlerTestHarness<T> Handler<T>(this BusTestHarness harness, MessageHandler<T> handler)
            where T : class
        {
            return new HandlerTestHarness<T>(harness, handler);
        }

        public static HandlerTestHarness<T> Handler<T>(this BusTestHarness harness)
            where T : class
        {
            return new HandlerTestHarness<T>(harness, context => Task.CompletedTask);
        }
    }
}

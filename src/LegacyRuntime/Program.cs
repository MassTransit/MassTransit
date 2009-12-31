namespace LegacyRuntime
{
    using Topshelf.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var cfg = RunnerConfigurator.New(c =>
                                                 {
                                                    c.ConfigureService<MassTransit.ServiceBus.LegacySubscriptionProxyService>("name", s=>
                                                                                   {
                                                                                       
                                                                                   });
                                                 });
        }
    }
}

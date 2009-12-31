namespace LegacyRuntime
{
    using MassTransit.LegacySupport;
    using Topshelf.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var cfg = RunnerConfigurator.New(c =>
                                                 {
                                                    c.ConfigureService<LegacySubscriptionProxyService>("name", s=>
                                                                                   {
                                                                                       
                                                                                   });
                                                 });
        }
    }
}

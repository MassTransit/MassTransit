namespace MassTransit.Host
{
    using System;
    using Logging;
    using Serilog;
    using SerilogIntegration;
    using Topshelf;
    using Topshelf.Logging;


    class Program
    {
        static Serilog.ILogger _baseLogger;

        static int Main()
        {
            SetupLogger();

            return (int)HostFactory.Run(x =>
            {
                x.SetStartTimeout(TimeSpan.FromMinutes(2));

                var configurator = new MassTransitHostConfigurator<MassTransitHostServiceBootstrapper>
                {
                    Description = "MassTransit Host - A service host for MassTransit endpoints",
                    DisplayName = "MassTransit Host",
                    ServiceName = "MassTransitHost"
                };

                configurator.Configure(x);
            });
        }

        static void SetupLogger()
        {
            _baseLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(@"log\MassTransit.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            SerilogLogWriterFactory.Use(_baseLogger);
            Logger.UseLogger(new SerilogLogger(_baseLogger));
        }
    }
}

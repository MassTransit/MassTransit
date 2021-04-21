namespace MassTransit.GrpcTransport.Testing
{
    using System.Collections.Generic;
    using Context;
    using Microsoft.Extensions.Logging;
    using Registration;


    public class GrpcTestHarnessRegistrationBusFactory :
        IRegistrationBusFactory
    {
        public IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications = null)
        {
            var testHarness = new GrpcTestHarness(specifications);

            testHarness.OnConfigureGrpcBus += configurator =>
            {
                var loggerFactory = context.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                    LogContext.ConfigureCurrentLogContext(loggerFactory);

                configurator.ConfigureEndpoints(context);
            };

            return new GrpcTestHarnessBusInstance(testHarness, context);
        }
    }
}

namespace MassTransit.Tests
{
    public interface ITestBusConfiguration
    {
        void ConfigureBus<T>(IBusRegistrationContext context, IBusFactoryConfigurator<T> configurator)
            where T : IReceiveEndpointConfigurator;
    }


    namespace Scenario
    {
        public class Json :
            ITestBusConfiguration
        {
            public void ConfigureBus<T>(IBusRegistrationContext context, IBusFactoryConfigurator<T> configurator)
                where T : IReceiveEndpointConfigurator
            {
            }
        }


        public class RawJson :
            ITestBusConfiguration
        {
            public void ConfigureBus<T>(IBusRegistrationContext context, IBusFactoryConfigurator<T> configurator)
                where T : IReceiveEndpointConfigurator
            {
                configurator.UseRawJsonSerializer();
            }
        }


        public class NewtonsoftJson :
            ITestBusConfiguration
        {
            public void ConfigureBus<T>(IBusRegistrationContext context, IBusFactoryConfigurator<T> configurator)
                where T : IReceiveEndpointConfigurator
            {
                configurator.UseNewtonsoftJsonSerializer();
            }
        }


        public class NewtonsoftRawJson :
            ITestBusConfiguration
        {
            public void ConfigureBus<T>(IBusRegistrationContext context, IBusFactoryConfigurator<T> configurator)
                where T : IReceiveEndpointConfigurator
            {
                configurator.UseNewtonsoftRawJsonSerializer();
            }
        }
    }
}

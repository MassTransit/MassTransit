namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using GreenPipes;


    public static class BusFactoryExtensions
    {
        public static IBusControl Build(this IBusFactory factory)
        {
            return factory.Build(Enumerable.Empty<ISpecification>());
        }

        public static IBusControl Build(this IBusFactory factory, IEnumerable<ISpecification> dependencies)
        {
            IEnumerable<ValidationResult> validationResult = factory.Validate()
                .Concat(dependencies.SelectMany(x => x.Validate()));

            var result = BusConfigurationResult.CompileResults(validationResult);

            try
            {
                return factory.CreateBus();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred during bus creation", ex);
            }
        }
    }
}

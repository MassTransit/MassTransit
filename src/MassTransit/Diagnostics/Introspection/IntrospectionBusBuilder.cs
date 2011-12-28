namespace MassTransit.Diagnostics.Introspection
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using BusConfigurators;
    using Configurators;

    public class IntrospectionBusBuilder : BusBuilderConfigurator
    {
        readonly Action<DiagnosticsProbe> _writeAction;

        public IntrospectionBusBuilder(Action<DiagnosticsProbe> action)
        {
            _writeAction = action;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_writeAction == null)
                yield return this.Failure("introspection", "You must provide a write method");
            else
                yield return this.Success("Introspection has been turned on.");
        }

        public BusBuilder Configure(BusBuilder builder)
        {
            var probe = new InMemoryDiagnosticsProbe();
            builder.AddBusServiceConfigurator(new IntrospectionServiceConfigurator());
            builder.AddPostCreateAction(bus=>
                {   

                    bus.Inspect(probe);
                    
                   


                    _writeAction(probe);
                });

            return builder;
        }

       
    }
}
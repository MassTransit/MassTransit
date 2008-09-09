namespace MassTransit.ServiceBus.Configuration
{
    using System;
    using System.Collections.Generic;

    public class ConfigureTheBus
    {
        private readonly BusOptions _options;
        private readonly IList<Uri> _registeredUris;

        public ConfigureTheBus(BusOptions options)
        {
            _registeredUris = new List<Uri>();
            _options = options;
        }

        public ConfigureTheBus ListensOn(string uri)
        {
            return ListensOn(new Uri(uri));
        }
        public ConfigureTheBus ListensOn(Uri uri)
        {
            _registeredUris.Add(uri);
            _options.ListensOn = uri;
            return this;
        }

        public ConfigureTheBus ReceivesCommandsOn(string uri)
        {
            return ReceivesCommandsOn(new Uri(uri));
        }
        public ConfigureTheBus ReceivesCommandsOn(Uri uri)
        {
            _registeredUris.Add(uri);
            _options.CommandedOn = uri;
            return this;
        }

        public ConfigureTheBus WithSharedSubscriptions(SubscriptionOptions subscriptionOptions)
        {
            _options.Subcriptions = subscriptionOptions;
            return this;
        }
        public ConfigureTheBus Using(SerializationOptions serializer)
        {
            _options.Serialization = serializer;
            return this;
        }
        public ConfigureTheBus AsACompetingConsumer()
        {
            _options.MarkAsCompetingConsumer();
            return this;
        }

        public ConfigureTheBus CommunicatesOn<T>() where T : IEndpoint
        {
            _options.RegisterTransport<T>();
            return this;
        }


        //last thing you call
        public BusOptions Validate()
        {
            //that all uris have transports
            //EndpointResolver.EnsureThatTransportsExist(_registeredUris);  


            //that the IoC is not null?
            //any custom options should be checked
            return _options;
        }

    }
}
namespace MassTransit.Host
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using nu.Utility;
    using ServiceBus;

    public class Controller
    {
        private readonly IArgumentParser _argumentParser = new ArgumentParser();
        private IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();

        private string _assemblyName;
        private string _endpointUri;
        private string _subscriptionEndpoint;

        [DefaultArgument(Required = true, AllowMultiple = true, Description = "The assemblies to load into the host process")]
        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        [Argument(Key = "endpoint", Required = false, Description = "The endpoint to use for the service bus")]
        public string EndpointUri
        {
            get { return _endpointUri; }
            set { _endpointUri = value; }
        }

        [Argument(Key = "subscription", Required = false, Description = "The remote subscription manager to use")]
        public string SubscriptionEndpoint
        {
            get { return _subscriptionEndpoint; }
            set { _subscriptionEndpoint = value; }
        }

        public void Start(string[] args)
        {
            IEnumerable<IArgument> arguments = _argumentParser.Parse(args);

            IArgumentMap mapper = _argumentMapFactory.CreateMap(this);

            mapper.ApplyTo(this, arguments);

            if (string.IsNullOrEmpty(_assemblyName))
            {
                Console.WriteLine("Usage: {0}", mapper.Usage);
                return;
            }

            LoadAssembly();
        }

        private void LoadAssembly()
        {
            try
            {
                Assembly assembly = Assembly.Load(_assemblyName);

                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (t.IsAssignableFrom(typeof (IAutoSubscriber)) && !t.IsAbstract)
                        Console.WriteLine("Type: {0}", t.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred: {0}", ex);
            }
        }
    }
}
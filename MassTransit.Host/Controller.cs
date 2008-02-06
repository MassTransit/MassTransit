namespace MassTransit.Host
{
    using System;
    using System.Collections.Generic;
    using nu.Utility;

    public class Controller
    {
        private readonly IArgumentParser _argumentParser = new ArgumentParser();
        private IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();

        private string _assemblyName;
        [DefaultArgument(Required = true, AllowMultiple = true, Description = "The assemblies to load into the host process")]
        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }


        public void Start(string[] args)
        {
            IEnumerable<IArgument> arguments = _argumentParser.Parse(args);

            IArgumentMap mapper = _argumentMapFactory.CreateMap(this);

            mapper.ApplyTo(this, arguments);

            if(string.IsNullOrEmpty(_assemblyName))
            {
                Console.WriteLine("Usage: {0}", mapper.Usage);
                return;
            }


        }
    }
}
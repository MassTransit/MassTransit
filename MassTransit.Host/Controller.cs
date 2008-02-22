namespace MassTransit.Host
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Config;
	using MassTransit.Host.Config.Util.Arguments;
	using ServiceBus;

	public class Controller
	{
		private readonly IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();
		private readonly IArgumentParser _argumentParser = new ArgumentParser();
		private IHostConfigurator _configurator;

		private string _ConfiguratorName;
		private bool _installService;
		private bool _isService;
		private bool _uninstallService;

		[Argument(Key = "config", Required = true, Description = "The configuration provider to use for the host")]
		public string Configurator
		{
			get { return _ConfiguratorName; }
			set { _ConfiguratorName = value; }
		}

		[Argument(Key = "install", Description = "Install the host service")]
		public bool InstallService
		{
			get { return _installService; }
			set { _installService = value; }
		}

		[Argument(Key = "service", Description = "Set when starting as a service")]
		public bool IsService
		{
			get { return _isService; }
			set { _isService = value; }
		}

		[Argument(Key = "uninstall", Description = "Uninstall the host service")]
		public bool UninstallService
		{
			get { return _uninstallService; }
			set { _uninstallService = value; }
		}

		public void Start(string[] args)
		{
			IEnumerable<IArgument> arguments = _argumentParser.Parse(args);

			IArgumentMap mapper = _argumentMapFactory.CreateMap(this);

			IEnumerable<IArgument> remaining = mapper.ApplyTo(this, arguments);

			if (string.IsNullOrEmpty(_ConfiguratorName))
			{
				Console.WriteLine("Usage: {0}", mapper.Usage);
			}
			else if (_installService)
			{
				RegisterService(arguments, true);
			}
			else if (_uninstallService)
			{
				RegisterService(arguments, false);
			}
			else if (_isService)
			{
				Console.WriteLine("Service not working yet.");
			}
			else
			{
				Console.WriteLine("Starting up as a console application");

				LoadConfiguration(remaining);

				if (_configurator != null)
				{
					foreach (IMessageService service in _configurator.Services)
					{
						service.Start();
					}

					Console.WriteLine("The service is running, press Control+C to exit.");

					Console.ReadKey();
				}
			}
		}

		private void RegisterService(IEnumerable<IArgument> arguments, bool install)
		{
			HostServiceInstaller installer = new HostServiceInstaller("MassTransitHost", "MassTransit Message Host", "Mass Transit Host");

			IArgumentMap installerMap = _argumentMapFactory.CreateMap(installer);
			installerMap.ApplyTo(installerMap, arguments);

			if (install)
			{
				Assembly assembly = Assembly.Load(_ConfiguratorName);

				installer.Register(new Assembly[] {assembly});
			}
			else
			{
				installer.Unregister();
			}
		}

		private void LoadConfiguration(IEnumerable<IArgument> arguments)
		{
			try
			{
				Assembly assembly = Assembly.Load(_ConfiguratorName);

				Type[] types = assembly.GetTypes();
				foreach (Type t in types)
				{
					if (t.IsAssignableFrom(typeof (IHostConfigurator)) && !t.IsAbstract)
					{
						Console.WriteLine("Type: {0}", t.Name);

						IHostConfigurator configurator = (IHostConfigurator) Activator.CreateInstance(t);
						if (configurator != null)
						{
							IArgumentMap mapper = _argumentMapFactory.CreateMap(configurator);

							mapper.ApplyTo(configurator, arguments);

							Configure(configurator);
							return;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception Occurred: {0}", ex);
			}
		}

		public void Configure(IHostConfigurator hostConfigurator)
		{
			_configurator = hostConfigurator;

			_configurator.Configure();
		}
	}
}
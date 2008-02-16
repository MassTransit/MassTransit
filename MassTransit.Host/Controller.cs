using System;
using System.Collections.Generic;
using System.Reflection;
using MassTransit.ServiceBus;
using nu.Utility;

namespace MassTransit.Host
{
	public class Controller
	{
		private readonly IArgumentMapFactory _argumentMapFactory = new ArgumentMapFactory();
		private readonly IArgumentParser _argumentParser = new ArgumentParser();

		private string _assemblyName;
		private bool _installService;
		private bool _isService;
		private bool _uninstallService;

		[DefaultArgument(Required = true, AllowMultiple = true, Description = "The assemblies to load into the host process")]
		public string AssemblyName
		{
			get { return _assemblyName; }
			set { _assemblyName = value; }
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

			mapper.ApplyTo(this, arguments);

			if (string.IsNullOrEmpty(_assemblyName))
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

				LoadAssembly();
			}
		}

		private void RegisterService(IEnumerable<IArgument> arguments, bool install)
		{
			HostServiceInstaller installer = new HostServiceInstaller("MassTransitHost", "MassTransit Message Host", "Mass Transit Host");

			IArgumentMap installerMap = _argumentMapFactory.CreateMap(installer);
			installerMap.ApplyTo(installerMap, arguments);

			if (install)
			{
				Assembly assembly = Assembly.Load(_assemblyName);

				installer.Register(new Assembly[] {assembly});
			}
			else
			{
				installer.Unregister();
			}
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

		public void Configure(IConfigurator configurator)
		{
			
		}
	}
}
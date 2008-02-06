using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
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
				return;
			}

			if (_installService)
			{
				RegisterService(arguments);
				return;
			}

			if (_uninstallService)
			{
				UnregisterService(arguments);
				return;
			}

			LoadAssembly();
		}

		private void UnregisterService(IEnumerable<IArgument> arguments)
		{
			HostServiceInstaller installer = new HostServiceInstaller("MassTransitHost", "MassTransit Message Host", "Mass Transit Host");

			IArgumentMap installerMap = _argumentMapFactory.CreateMap(installer);
			IEnumerable<IArgument> remaining = installerMap.ApplyTo(installerMap, arguments);

			if (!installer.IsInstalled())
			{
				Console.WriteLine("Service is not installed");
				return;
			}

			using (TransactedInstaller ti = new TransactedInstaller())
			{
				ti.Installers.Add(installer);

				string path = string.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
				string[] commandLine = {path};

				InstallContext context = new InstallContext(null, commandLine);
				ti.Context = context;

				ti.Uninstall(null);
			}
		}

		private void RegisterService(IEnumerable<IArgument> arguments)
		{
			HostServiceInstaller installer = new HostServiceInstaller("MassTransitHost", "MassTransit Message Host", "Mass Transit Host");

			IArgumentMap installerMap = _argumentMapFactory.CreateMap(installer);
			IEnumerable<IArgument> remaining = installerMap.ApplyTo(installerMap, arguments);

			if (installer.IsInstalled())
			{
				Console.WriteLine("Service is already installed");
				return;
			}

			Assembly assembly = Assembly.Load(_assemblyName);
			installer.AddAssembly(assembly);

			using (TransactedInstaller ti = new TransactedInstaller())
			{
				ti.Installers.Add(installer);

				string path = string.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
				string[] commandLine = {path};

				InstallContext context = new InstallContext(null, commandLine);
				ti.Context = context;

				Hashtable savedState = new Hashtable();

				ti.Install(savedState);
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
	}
}
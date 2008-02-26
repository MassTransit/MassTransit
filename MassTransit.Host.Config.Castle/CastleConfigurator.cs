using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using MassTransit.Host.Config.Util.Arguments;
using MassTransit.ServiceBus;

namespace MassTransit.Host.Config.Castle
{
	public class CastleConfigurator :
		IHostConfigurator
	{
		private string _configFile;
		private IWindsorContainer _container;
		private List<IMessageService> _services;

		[Argument(Required = true, Key = "file", Description = "Configuration file name")]
		public string ConfigFile
		{
			get { return _configFile; }
			set { _configFile = value; }
		}

		#region IHostConfigurator Members

		public void Configure()
		{
			_container = new WindsorContainer(new XmlInterpreter(_configFile));

			_services = new List<IMessageService>();
			IHandler[] handlers = _container.Kernel.GetAssignableHandlers(typeof (IMessageService));
			if (handlers != null)
			{
				foreach (IHandler handler in handlers)
				{
					IMessageService service = (IMessageService) handler.Resolve(CreationContext.Empty);
					if (service != null)
					{
						_services.Add(service);
					}
				}
			}
		}

		public IEnumerable<IMessageService> Services
		{
			get { return _services; }
		}

		#endregion

		public void Dispose()
		{
			foreach (IMessageService service in _services)
			{
				service.Dispose();
			}

			_services.Clear();
		}
	}
}
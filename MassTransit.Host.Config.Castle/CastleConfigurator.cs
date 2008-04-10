using CreationContext=Castle.MicroKernel.CreationContext;
using IHandler=Castle.MicroKernel.IHandler;
using IWindsorContainer=Castle.Windsor.IWindsorContainer;
using WindsorContainer=Castle.Windsor.WindsorContainer;
using XmlInterpreter=Castle.Windsor.Configuration.Interpreters.XmlInterpreter;

namespace MassTransit.Host.Config.Castle
{
    using System.Collections.Generic;
    using MassTransit.Host.Config.Util.Arguments;
    using ServiceBus;

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
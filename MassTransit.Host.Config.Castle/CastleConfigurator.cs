using CreationContext=Castle.MicroKernel.CreationContext;
using IHandler=Castle.MicroKernel.IHandler;
using IWindsorContainer=Castle.Windsor.IWindsorContainer;
using WindsorContainer=Castle.Windsor.WindsorContainer;
using XmlInterpreter=Castle.Windsor.Configuration.Interpreters.XmlInterpreter;

namespace MassTransit.Host.Config.Castle
{
    using System.Collections.Generic;
    using MassTransit.Host.Config.Util.Arguments;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;

    public class CastleConfigurator :
        IHostConfigurator
    {
        private string _configFile;
        private IWindsorContainer _container;
        private List<IHostedService> _services;

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

            _services = new List<IHostedService>();
            IHandler[] handlers = _container.Kernel.GetAssignableHandlers(typeof (IHostedService));
            if (handlers != null)
            {
                foreach (IHandler handler in handlers)
                {
                    IHostedService service = (IHostedService) handler.Resolve(CreationContext.Empty);
                    if (service != null)
                    {
                        _services.Add(service);
                    }
                }
            }

            SortSubscriptionClientToTheTop();
        }

        //TODO: HACK!!!!
        private void SortSubscriptionClientToTheTop()
        {
            foreach (IHostedService service in _services)
            {
                if(service is SubscriptionClient)
                {
                    _services.Remove(service);
                    _services.Insert(0, service);
                    
                    break;
                }
            }
            
        }

        public IEnumerable<IHostedService> Services
        {
            get { return _services; }
        }

        #endregion

        public void Dispose()
        {
            foreach (IHostedService service in _services)
            {
                service.Dispose();
            }

            _services.Clear();
        }
    }
}
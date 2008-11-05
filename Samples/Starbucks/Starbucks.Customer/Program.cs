using System;
using System.Windows.Forms;
using Castle.Windsor;
using MassTransit.Host;
using MassTransit.Host.Actions;
using MassTransit.Host.Configurations;
using MassTransit.Host.LifeCycles;
using MassTransit.WindsorIntegration;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Customer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Customer.Castle.xml");
            var builder = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => builder);

            container.AddComponent<Form, OrderDrinkForm>();

            //do container SHIT
            IInstallationConfiguration config = new CustomerConfiguration(ServiceLocator.Current);
            Runner.Run(config, args);
        }
    }

    public class CustomerConfiguration : LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public CustomerConfiguration(IServiceLocator serviceLocator)
        {
            _lifecycle = new CustomerLifecycle(serviceLocator);
        }        

        public override string[] Dependencies
        {
            get { return new[] {KnownServiceNames.Msmq}; }
        }

        public override string ServiceName
        {
            get { return "StarbucksCustomer"; }
        }

        public override string DisplayName
        {
            get { return "Startbucks Customer"; }
        }

        public override string Description
        {
            get { return "a Mass Transit sample service for ordering coffee."; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }
    }

    public class CustomerLifecycle : HostedLifeCycle
    {
        public CustomerLifecycle(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override NamedAction DefaultAction
        {
            get
            {
                return NamedAction.Gui;
            }
        }

        public override void Start()
        {            
        }

        public override void Stop()
        {         
        }
    }
}

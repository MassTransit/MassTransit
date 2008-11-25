namespace Client
{
    using System;
    using MassTransit;
    using MassTransit.Host.LifeCycles;
    using Microsoft.Practices.ServiceLocation;
    using SecurityMessages;

    public class ClientLifeCycle :
        HostedLifecycle
    {
        private IServiceBus _bus;

        public ClientLifeCycle(IServiceLocator serviceLocator) : base(serviceLocator)
        {
        }

        public override void Start()
        {
            _bus = base.ServiceLocator.GetInstance<IServiceBus>();

            _bus.Subscribe<PasswordUpdater>();

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));

            var message = new RequestPasswordUpdate(newPassword);

            _bus.Publish(message);

            Console.WriteLine("Waiting For Reply");
            Console.WriteLine(new string('-', 20));
        }

        public override void Stop()
        {
            //do nothing
        }
    }
}
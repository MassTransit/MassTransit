namespace Client
{
    using System;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using SecurityMessages;

    public class ClientLifeCycle :
        HostedLifeCycle
    {
        private IServiceBus _bus;

        public ClientLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.AddComponent<PasswordUpdater>();

            _bus = Container.Resolve<IServiceBus>();

            _bus.AddComponent<PasswordUpdater>();

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
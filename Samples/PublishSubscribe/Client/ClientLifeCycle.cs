namespace Client
{
    using System;
    using MassTransit;
    using Microsoft.Practices.ServiceLocation;
    using SecurityMessages;

    public class ClientLifeCycle
    {
        private IServiceBus _bus;

        public void Start()
        {
            _bus = ServiceLocator.Current.GetInstance<IServiceBus>();

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

        public void Stop()
        {
            //do nothing
        }
    }
}
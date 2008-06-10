namespace Client
{
    using System;
    using MassTransit.ServiceBus;
    using SecurityMessages;

    public class AskPasswordQuestion :
        IHostedService
    {
        private IServiceBus _bus;


        public AskPasswordQuestion(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _bus.AddComponent<PasswordUpdater>();

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));
            _bus.Publish(new RequestPasswordUpdate(newPassword));

            Console.WriteLine("Waiting For Reply");
            Console.WriteLine(new string('-', 20));
            Console.ReadKey();
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}
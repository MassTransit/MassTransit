namespace Client
{
    using System;
    using Castle.Windsor;
    using log4net.Config;
    using MassTransit.ServiceBus;
    using MassTransit.WindsorIntegration;
    using SecurityMessages;

    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            IWindsorContainer container = new DefaultMassTransitContainer("castle.xml");
            
            IServiceBus bus = container.Resolve<IServiceBus>();
            bus.AddComponent<PasswordUpdater>();

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));
            bus.Publish(new RequestPasswordUpdate(newPassword));

            Console.WriteLine("Waiting For Reply");
            Console.WriteLine(new string('-', 20));
            Console.ReadKey();

            container.Dispose();
        }

        public class PasswordUpdater :
            Consumes<PasswordUpdateComplete>.All
        {
            public void Consume(PasswordUpdateComplete message)
            {
                Console.WriteLine("Password Set!");
                Console.WriteLine("Thank You. Press any key to exit");
            }

        }
    }
}
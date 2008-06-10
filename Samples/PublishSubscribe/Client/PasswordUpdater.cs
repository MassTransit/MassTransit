namespace Client
{
    using System;
    using MassTransit.ServiceBus;
    using SecurityMessages;

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
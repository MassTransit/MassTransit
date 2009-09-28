namespace CodeCamp.Service
{
    using System;
    using MassTransit;
    using Messages;

    public class Responder :
        Consumes<UserPasswordSuccess>.All,
        Consumes<UserPasswordFailure>.All
    {
        public void Consume(UserPasswordFailure message)
        {
            Console.WriteLine("Audit: Failed user password check for user {0} at {1}", message.Username, message.TimeStamp);
        }

        public void Consume(UserPasswordSuccess message)
        {
            Console.WriteLine("Audit: User password succeeded password check for user {0} at {1}", message.Username, message.TimeStamp);
        }
    }
}
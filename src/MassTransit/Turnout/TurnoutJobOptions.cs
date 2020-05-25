namespace MassTransit.Turnout
{
    using System;
    using ConsumeConfigurators;


    public class TurnoutJobOptions<TJob> :
        IOptions
        where TJob : class
    {
        public TurnoutJobOptions()
        {
            JobTimeout = TimeSpan.FromMinutes(5);
        }

        public TimeSpan JobTimeout { get; set; }

        public int ConcurrentJobLimit { get; set; }
    }
}

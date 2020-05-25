namespace MassTransit.Turnout.Configuration
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;


    public class TurnoutJobConfigurator<TJob> :
        ITurnoutJobConfigurator<TJob>,
        ISpecification
        where TJob : class
    {
        public TurnoutJobConfigurator()
        {
            ConcurrentJobLimit = 1;
            JobTimeout = TimeSpan.FromMinutes(30);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (ConcurrentJobLimit < 1)
                yield return this.Failure("ConcurrentJobLimit", "must be >= 1");
            if (JobTimeout < TimeSpan.FromSeconds(60))
                yield return this.Failure("JobTimeout", "must be >= 1 minute");
            if (JobFactory == null)
                yield return this.Failure("JobFactory", "must be specified");
        }

        public int ConcurrentJobLimit { get; set; }
        public TimeSpan JobTimeout { get; set; }
        public IJobFactory<TJob> JobFactory { get; set; }
    }
}

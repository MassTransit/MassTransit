namespace MassTransit.EntityFrameworkCoreIntegration.Turnout
{
    using System.Collections.Generic;
    using Mappings;
    using Microsoft.EntityFrameworkCore;


    public class TurnoutSagaDbContext :
        SagaDbContext
    {
        public TurnoutSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new TurnoutJobTypeStateSagaMap();
                yield return new TurnoutJobStateSagaMap();
                yield return new TurnoutJobAttemptStateSagaMap();
            }
        }
    }
}

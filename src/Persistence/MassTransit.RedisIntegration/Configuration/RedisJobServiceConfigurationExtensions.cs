namespace MassTransit.RedisIntegration.Configuration
{
    using JobService.Components.StateMachines;
    using JobService.Configuration;
    using StackExchange.Redis;


    public static class RedisJobServiceConfigurationExtensions
    {
        public static void UseRedisSagaRepository(this IJobServiceConfigurator configurator, IDatabase redisDB)
        {
            configurator.Repository = RedisSagaRepository<JobTypeSaga>.Create(() => redisDB, true, keyPrefix: "jobtype");

            configurator.JobRepository = RedisSagaRepository<JobSaga>.Create(() => redisDB, true, keyPrefix: "job");
            
            configurator.JobAttemptRepository = RedisSagaRepository<JobAttemptSaga>.Create(() => redisDB, true, keyPrefix: "jobattempt");
        }
    }
}

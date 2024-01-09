namespace MassTransit
{
    using System;
    using SqlTransport.Configuration;


    public static class SqlScheduleMessageExtensions
    {
        /// <summary>
        /// Uses the database transport's built-in message scheduler
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseDbMessageScheduler(this IBusFactoryConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var pipeBuilderConfigurator = new SqlMessageSchedulerSpecification();

            configurator.AddPrePipeSpecification(pipeBuilderConfigurator);
        }
    }
}

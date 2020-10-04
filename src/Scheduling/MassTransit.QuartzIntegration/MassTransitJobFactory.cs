using Microsoft.Extensions.Options;

namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using Newtonsoft.Json;
    using Quartz;
    using Quartz.Spi;


    public class MassTransitJobFactory :
        IJobFactory
    {
        readonly IServiceProvider _serviceProvider;
        readonly IOptions<QuartzOptions> _options;
        readonly ConcurrentDictionary<Type, IJobFactory> _typeFactories;

        public MassTransitJobFactory(IServiceProvider serviceProvider, IOptions<QuartzOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _typeFactories = new ConcurrentDictionary<Type, IJobFactory>();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            if (jobDetail == null)
                throw new SchedulerException("JobDetail was null");

            var type = jobDetail.JobType;

            return _typeFactories.GetOrAdd(type, CreateJobFactory)
                .NewJob(bundle, scheduler);
        }

        public void ReturnJob(IJob job)
        {
        }

        IJobFactory CreateJobFactory(Type type)
        {
            var genericType = typeof(MassTransitJobFactory<>).MakeGenericType(type);
            return (IJobFactory) Activator.CreateInstance(genericType, _serviceProvider, _options);
        }
    }


    public class MassTransitJobFactory<T> :
        MicrosoftDependencyInjectionJobFactory
        where T : IJob
    {
        public MassTransitJobFactory(IServiceProvider serviceProvider, IOptions<QuartzOptions> options) : base(serviceProvider, options)
        {
        }

        protected override JobDataMap BuildJobDataMap(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobData = base.BuildJobDataMap(bundle, scheduler);
            jobData.Put("PayloadMessageHeadersAsJson", CreatePayloadHeaderString(bundle));
            return jobData;
        }

        protected override void SetJobProperty(IJob job, string name, object value)
        {
            if (TypeCache<T>.ReadWritePropertyCache.TryGetProperty(name, out ReadWriteProperty<T> property))
            {
                if (property.Property.PropertyType == typeof(Uri))
                    value = new Uri(value.ToString());

                property.Set((T) job, value);
            }
        }

        /// <summary>
        /// Some additional properties from the TriggerFiredBundle
        /// There is a bug in RabbitMq.Client that prevents using the DateTimeOffset type in the headers
        /// These values are being serialized as ISO-8601 round trip string
        /// </summary>
        /// <param name="bundle"></param>
        string CreatePayloadHeaderString(TriggerFiredBundle bundle)
        {
            var timeHeaders = new Dictionary<string, object> {{MessageHeaders.Quartz.Sent, bundle.FireTimeUtc}};
            if (bundle.ScheduledFireTimeUtc.HasValue)
                timeHeaders.Add(MessageHeaders.Quartz.Scheduled, bundle.ScheduledFireTimeUtc);

            if (bundle.NextFireTimeUtc.HasValue)
                timeHeaders.Add(MessageHeaders.Quartz.NextScheduled, bundle.NextFireTimeUtc);

            if (bundle.PrevFireTimeUtc.HasValue)
                timeHeaders.Add(MessageHeaders.Quartz.PreviousSent, bundle.PrevFireTimeUtc);

            if (bundle.JobDetail.JobDataMap.TryGetValue("TokenId", out var tokenId))
                timeHeaders.Add(MessageHeaders.SchedulingTokenId, tokenId);

            return JsonConvert.SerializeObject(timeHeaders);
        }
    }
}

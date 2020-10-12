namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Generic;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using Metadata;
    using Newtonsoft.Json;
    using Quartz;
    using Quartz.Simpl;
    using Quartz.Spi;


    public class MassTransitJobFactory :
        IJobFactory
    {
        readonly IBus _bus;
        readonly IJobFactory _jobFactory;

        public MassTransitJobFactory(IBus bus, IJobFactory jobFactory)
        {
            _bus = bus;
            _jobFactory = jobFactory ?? new PropertySettingJobFactory();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            if (jobDetail == null)
                throw new SchedulerException("JobDetail was null");

            var type = jobDetail.JobType;
            if (type == typeof(ScheduledMessageJob))
            {
                try
                {
                    var job = new ScheduledMessageJob(_bus);

                    var jobData = new JobDataMap();
                    jobData.PutAll(scheduler.Context);
                    jobData.PutAll(bundle.JobDetail.JobDataMap);
                    jobData.PutAll(bundle.Trigger.JobDataMap);
                    jobData.Put("PayloadMessageHeadersAsJson", CreatePayloadHeaderString(bundle));

                    SetObjectProperties(job, jobData);

                    return job;
                }
                catch (Exception ex)
                {
                    throw new SchedulerException($"Problem instantiating class '{TypeMetadataCache.GetShortName(bundle.JobDetail.JobType)}'", ex);
                }
            }

            return _jobFactory.NewJob(bundle, scheduler);
        }

        public void ReturnJob(IJob job)
        {
            _jobFactory.ReturnJob(job);
        }

        static void SetObjectProperties(ScheduledMessageJob job, JobDataMap jobData)
        {
            foreach (var key in jobData.Keys)
            {
                if (TypeCache<ScheduledMessageJob>.ReadWritePropertyCache.TryGetProperty(key, out ReadWriteProperty<ScheduledMessageJob> property))
                {
                    var value = jobData[key];

                    if (property.Property.PropertyType == typeof(Uri))
                        value = new Uri(value.ToString());

                    property.Set(job, value);
                }
            }
        }

        /// <summary>
        /// Some additional properties from the TriggerFiredBundle
        /// There is a bug in RabbitMq.Client that prevents using the DateTimeOffset type in the headers
        /// These values are being serialized as ISO-8601 round trip string
        /// </summary>
        /// <param name="bundle"></param>
        static string CreatePayloadHeaderString(TriggerFiredBundle bundle)
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

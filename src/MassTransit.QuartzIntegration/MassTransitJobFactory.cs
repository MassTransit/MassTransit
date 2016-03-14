// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals.Extensions;
    using Internals.Reflection;
    using Quartz;
    using Quartz.Spi;


    public class MassTransitJobFactory :
        IJobFactory
    {
        readonly IBus _bus;
        readonly ConcurrentDictionary<Type, IJobFactory> _typeFactories;

        public MassTransitJobFactory(IBus bus)
        {
            _bus = bus;
            _typeFactories = new ConcurrentDictionary<Type, IJobFactory>();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            IJobDetail jobDetail = bundle.JobDetail;
            if (jobDetail == null)
                throw new SchedulerException("JobDetail was null");

            Type type = jobDetail.JobType;

            return _typeFactories.GetOrAdd(type, CreateJobFactory)
                .NewJob(bundle, scheduler);
        }

        public void ReturnJob(IJob job)
        {
        }

        IJobFactory CreateJobFactory(Type type)
        {
            Type genericType = typeof(MassTransitJobFactory<>).MakeGenericType(type);

            return (IJobFactory)Activator.CreateInstance(genericType, _bus);
        }
    }


    public class MassTransitJobFactory<T> :
        IJobFactory
        where T : IJob
    {
        readonly IBus _bus;
        readonly Func<IBus, T> _factory;
        readonly ReadWritePropertyCache<T> _propertyCache;

        public MassTransitJobFactory(IBus bus)
        {
            _bus = bus;
            _factory = CreateConstructor();

            _propertyCache = new ReadWritePropertyCache<T>(true);
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                T job = _factory(_bus);

                var jobData = new JobDataMap();
                jobData.PutAll(scheduler.Context);
                jobData.PutAll(bundle.JobDetail.JobDataMap);
                jobData.PutAll(bundle.Trigger.JobDataMap);
                jobData.Put("HeadersAsJson", CreateFireTimeHeaders(bundle));

                SetObjectProperties(job, jobData);

                return job;
            }
            catch (Exception ex)
            {
                var sex = new SchedulerException(string.Format(CultureInfo.InvariantCulture,
                    "Problem instantiating class '{0}'", bundle.JobDetail.JobType.FullName), ex);
                throw sex;
            }
        }

        public void ReturnJob(IJob job)
        {
        }

        void SetObjectProperties(T job, JobDataMap jobData)
        {
            foreach (string key in jobData.Keys)
            {
                ReadWriteProperty<T> property;
                if (_propertyCache.TryGetProperty(key, out property))
                {
                    object value = jobData[key];

                    if (property.Property.PropertyType == typeof(Uri))
                        value = new Uri(value.ToString());

                    property.Set(job, value);
                }
            }
        }

        Func<IBus, T> CreateConstructor()
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(new[] { typeof(IBus) });
            if (ctor != null)
                return CreateServiceBusConstructor(ctor);

            ctor = typeof(T).GetConstructor(Type.EmptyTypes);
            if (ctor != null)
                return CreateDefaultConstructor(ctor);

            throw new SchedulerException(string.Format(CultureInfo.InvariantCulture,
                "The job class does not have a supported constructor: {0}", typeof(T).GetTypeName()));
        }

        Func<IBus, T> CreateDefaultConstructor(ConstructorInfo constructorInfo)
        {
            ParameterExpression bus = Expression.Parameter(typeof(IBus), "bus");
            NewExpression @new = Expression.New(constructorInfo);

            return Expression.Lambda<Func<IBus, T>>(@new, bus).Compile();
        }

        Func<IBus, T> CreateServiceBusConstructor(ConstructorInfo constructorInfo)
        {
            ParameterExpression bus = Expression.Parameter(typeof(IBus), "bus");
            NewExpression @new = Expression.New(constructorInfo, bus);

            return Expression.Lambda<Func<IBus, T>>(@new, bus).Compile();
        }

        /// <summary>
        /// Some additional properties from the TriggerFiredBundle
        /// </summary>
        /// <param name="bundle"></param>
        public string CreateFireTimeHeaders(TriggerFiredBundle bundle)
        {
            var timeHeaders = new Dictionary<string, DateTimeOffset?>();
            timeHeaders.Add("ScheduledFireTimeUtc", bundle.ScheduledFireTimeUtc);
            timeHeaders.Add("FireTimeUtc", bundle.FireTimeUtc);
            timeHeaders.Add("NextFireTimeUtc", bundle.NextFireTimeUtc);
            timeHeaders.Add("PrevFireTimeUtc", bundle.PrevFireTimeUtc);

            return JsonConvert.SerializeObject(timeHeaders);


        }
    }
}
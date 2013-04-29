// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Monitoring
{
    using System;
    using Logging;

    public class ServiceBusInstancePerformanceCounters :
        IDisposable
    {
        static readonly ILog _log = Logger.Get(typeof (ServiceBusInstancePerformanceCounters));

        bool _disposed;

        public ServiceBusInstancePerformanceCounters(string instanceName)
        {
            InstanceName = instanceName;

            ConsumerThreadCount = CreateCounter(ServiceBusPerformanceCounters.Instance.ConsumerThreadCount);
            ReceiveThreadCount = CreateCounter(ServiceBusPerformanceCounters.Instance.ReceiveThreadCount);
            ReceiveRate = CreateCounter(ServiceBusPerformanceCounters.Instance.ReceiveRate);
            PublishRate = CreateCounter(ServiceBusPerformanceCounters.Instance.PublishRate);
            SendRate = CreateCounter(ServiceBusPerformanceCounters.Instance.SendRate);
            ReceiveCount = CreateCounter(ServiceBusPerformanceCounters.Instance.ReceiveCount);
            PublishCount = CreateCounter(ServiceBusPerformanceCounters.Instance.PublishCount);
            SentCount = CreateCounter(ServiceBusPerformanceCounters.Instance.SentCount);
            ConsumerDuration = CreateCounter(ServiceBusPerformanceCounters.Instance.ConsumerDuration);
            ConsumerDurationBase = CreateCounter(ServiceBusPerformanceCounters.Instance.ConsumerDurationBase);
            ReceiveDuration = CreateCounter(ServiceBusPerformanceCounters.Instance.ReceiveDuration);
            ReceiveDurationBase = CreateCounter(ServiceBusPerformanceCounters.Instance.ReceiveDurationBase);
            PublishDuration = CreateCounter(ServiceBusPerformanceCounters.Instance.PublishDuration);
            PublishDurationBase = CreateCounter(ServiceBusPerformanceCounters.Instance.PublishDurationBase);
        }

        public IPerformanceCounter ConsumerDuration { get; private set; }
        public IPerformanceCounter ConsumerDurationBase { get; private set; }

        public IPerformanceCounter ConsumerThreadCount { get; private set; }
        public string InstanceName { get; private set; }
        public IPerformanceCounter PublishCount { get; private set; }
        public IPerformanceCounter PublishDuration { get; private set; }
        public IPerformanceCounter PublishDurationBase { get; private set; }
        public IPerformanceCounter PublishRate { get; private set; }

        public IPerformanceCounter ReceiveCount { get; private set; }

        public IPerformanceCounter ReceiveDuration { get; private set; }
        public IPerformanceCounter ReceiveDurationBase { get; private set; }
        public IPerformanceCounter ReceiveRate { get; private set; }
        public IPerformanceCounter ReceiveThreadCount { get; private set; }
        public IPerformanceCounter SendRate { get; private set; }
        public IPerformanceCounter SentCount { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Close()
        {
            if (ConsumerThreadCount != null)
            {
                ConsumerThreadCount.Dispose();
                ConsumerThreadCount = null;
            }
            if (ReceiveThreadCount != null)
            {
                ReceiveThreadCount.Dispose();
                ReceiveThreadCount = null;
            }
            if (ReceiveRate != null)
            {
                ReceiveRate.Dispose();
                ReceiveRate = null;
            }
            if (PublishRate != null)
            {
                PublishRate.Dispose();
                PublishRate = null;
            }
            if (SendRate != null)
            {
                SendRate.Dispose();
                SendRate = null;
            }
            if (ReceiveCount != null)
            {
                ReceiveCount.Dispose();
                ReceiveCount = null;
            }
            if (PublishCount != null)
            {
                PublishCount.Dispose();
                PublishCount = null;
            }
            if (SentCount != null)
            {
                SentCount.Dispose();
                SentCount = null;
            }
            if (ConsumerDuration != null)
            {
                ConsumerDuration.Dispose();
                ConsumerDuration = null;
            }
            if (ConsumerDurationBase != null)
            {
                ConsumerDurationBase.Dispose();
                ConsumerDurationBase = null;
            }
            if (ReceiveDuration != null)
            {
                ReceiveDuration.Dispose();
                ReceiveDuration = null;
            }
            if (ReceiveDurationBase != null)
            {
                ReceiveDurationBase.Dispose();
                ReceiveDurationBase = null;
            }
            if (PublishDuration != null)
            {
                PublishDuration.Dispose();
                PublishDuration = null;
            }
            if (PublishDurationBase != null)
            {
                PublishDurationBase.Dispose();
                PublishDurationBase = null;
            }
        }

        IPerformanceCounter CreateCounter(RuntimePerformanceCounter counter)
        {
            try
            {
                return new InstancePerformanceCounter(counter.Name,
                    ServiceBusPerformanceCounters.CategoryName,
                    InstanceName);
            }
            catch (InvalidOperationException ex)
            {
                _log.Warn("Unable to create performance counter", ex);

                return new NullPerformanceCounter();
            }
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                Close();
            }

            _disposed = true;
        }
    }
}
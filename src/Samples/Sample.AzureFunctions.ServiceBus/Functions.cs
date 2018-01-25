// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Sample.AzureFunctions.ServiceBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.WebJobs.ServiceBusIntegration;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Microsoft.ServiceBus.Messaging;


    public static class Functions
    {
        [FunctionName("SubmitOrder")]
        public static Task SubmitOrderAsync([ServiceBusTrigger("input-queue", AccessRights.Manage)]
            BrokeredMessage message, IBinder binder,
            TraceWriter traceWriter, CancellationToken cancellationToken)
        {
            traceWriter.Info("Creating brokered message receiver");

            var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder, cfg =>
            {
                cfg.CancellationToken = cancellationToken;
                cfg.SetLog(traceWriter);
                cfg.InputAddress = new Uri("sb://masstransit-build.servicebus.windows.net/input-queue");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(cfg.Log));
            });

            return handler.Handle(message);
        }

        [FunctionName("AuditOrder")]
        public static Task AuditOrderAsync([EventHubTrigger("input-hub")] EventData message, IBinder binder,
            TraceWriter traceWriter, CancellationToken cancellationToken)
        {
            traceWriter.Info("Creating EventHub receiver");

            var handler = Bus.Factory.CreateEventDataReceiver(binder, cfg =>
            {
                cfg.CancellationToken = cancellationToken;
                cfg.SetLog(traceWriter);
                cfg.InputAddress = new Uri("sb://masstransit-eventhub.servicebus.windows.net/input-hub");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer<AuditOrderConsumer>(() => new AuditOrderConsumer(cfg.Log));
            });

            return handler.Handle(message);
        }
    }


    public class SubmitOrderConsumer :
        IConsumer<SubmitOrder>
    {
        readonly ILog _log;

        public SubmitOrderConsumer(ILog log)
        {
            _log = log;
        }

        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Processing Order: {0}", context.Message.OrderNumber);

            context.Publish<OrderReceived>(new
            {
                context.Message.OrderNumber,
                Timestamp = DateTime.UtcNow,
            });

            return context.RespondAsync<OrderAccepted>(new {context.Message.OrderNumber});
        }
    }


    public class AuditOrderConsumer :
        IConsumer<OrderReceived>
    {
        readonly ILog _log;

        public AuditOrderConsumer(ILog log)
        {
            _log = log;
        }

        public async Task Consume(ConsumeContext<OrderReceived> context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Received Order: {0}", context.Message.OrderNumber);
        }
    }


    public interface SubmitOrder
    {
        string OrderNumber { get; }
    }


    public interface OrderAccepted
    {
        string OrderNumber { get; }
    }


    public interface OrderReceived
    {
        DateTime Timestamp { get; }

        string OrderNumber { get; }
    }
}
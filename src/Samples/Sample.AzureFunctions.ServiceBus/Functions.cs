namespace Sample.AzureFunctions.ServiceBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Context;
    using MassTransit.WebJobs.ServiceBusIntegration;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;


    public static class Functions
    {
        [FunctionName("SubmitOrder")]
        public static Task SubmitOrderAsync([ServiceBusTrigger("input-queue")] Message message, IBinder binder, Microsoft.Extensions.Logging.ILogger logger,
            CancellationToken cancellationToken)
        {
            LogContext.ConfigureCurrentLogContext(logger);

            LogContext.Info?.Log("Creating brokered message receiver");

            var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder, cfg =>
            {
                cfg.CancellationToken = cancellationToken;
                cfg.InputAddress = new Uri("sb://masstransit-build.servicebus.windows.net/input-queue");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer(() => new SubmitOrderConsumer());
            });

            return handler.Handle(message);
        }

        [FunctionName("AuditOrder")]
        public static Task AuditOrderAsync([EventHubTrigger("input-hub")] EventData message, IBinder binder, Microsoft.Extensions.Logging.ILogger logger,
            CancellationToken cancellationToken)
        {
            LogContext.ConfigureCurrentLogContext(logger);

            LogContext.Info?.Log("Creating event hub receiver");

            var handler = Bus.Factory.CreateEventDataReceiver(binder, cfg =>
            {
                cfg.CancellationToken = cancellationToken;
                cfg.InputAddress = new Uri("sb://masstransit-eventhub.servicebus.windows.net/input-hub");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer(() => new AuditOrderConsumer());
            });

            return handler.Handle(message);
        }
    }


    public class SubmitOrderConsumer :
        IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            LogContext.Debug?.Log("Processing Order: {OrderNumber}", context.Message.OrderNumber);

            context.Publish<OrderReceived>(new
            {
                context.Message.OrderNumber,
                Timestamp = DateTime.UtcNow
            });

            return context.RespondAsync<OrderAccepted>(new {context.Message.OrderNumber});
        }
    }


    public class AuditOrderConsumer :
        IConsumer<OrderReceived>
    {
        public async Task Consume(ConsumeContext<OrderReceived> context)
        {
            LogContext.Debug?.Log("Received Order: {OrderNumber}", context.Message.OrderNumber);
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

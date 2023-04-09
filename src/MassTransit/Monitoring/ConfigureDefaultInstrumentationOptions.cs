namespace MassTransit.Monitoring
{
    using Metadata;
    using Microsoft.Extensions.Options;


    public class ConfigureDefaultInstrumentationOptions :
        IConfigureOptions<InstrumentationOptions>
    {
        public void Configure(InstrumentationOptions options)
        {
            options.ServiceName = HostMetadataCache.Host.ProcessName;
            options.EndpointLabel = "messaging.masstransit.destination";
            options.ConsumerTypeLabel = "messaging.masstransit.consumer_type";
            options.ExceptionTypeLabel = "messaging.masstransit.exception_type";
            options.MessageTypeLabel = "messaging.masstransit.message_type";
            options.ActivityNameLabel = "messaging.masstransit.activity_type";
            options.ArgumentTypeLabel = "messaging.masstransit.argument_type";
            options.LogTypeLabel = "messaging.masstransit.log_type";
            options.ServiceNameLabel = "messaging.masstransit.service";
            options.ReceiveTotal = "messaging.masstransit.receive";
            options.ReceiveFaultTotal = "messaging.masstransit.receive.errors";
            options.ReceiveDuration = "messaging.masstransit.receive.duration";
            options.ReceiveInProgress = "messaging.masstransit.receive.active";
            options.ConsumeTotal = "messaging.masstransit.consume";
            options.ConsumeFaultTotal = "messaging.masstransit.consume.errors";
            options.ConsumeRetryTotal = "messaging.masstransit.consume.retries";
            options.ConsumeDuration = "messaging.masstransit.consume.duration";
            options.ConsumerInProgress = "messaging.masstransit.consume.active";
            options.SagaTotal = "messaging.masstransit.saga";
            options.SagaFaultTotal = "messaging.masstransit.saga.errors";
            options.SagaDuration = "messaging.masstransit.saga.duration";
            options.HandlerTotal = "messaging.masstransit.handler";
            options.HandlerFaultTotal = "messaging.masstransit.handler.errors";
            options.HandlerDuration = "messaging.masstransit.handler.duration";
            options.OutboxDeliveryTotal = "messaging.masstransit.outbox.delivery";
            options.OutboxDeliveryFaultTotal = "messaging.masstransit.outbox.delivery.errors";
            options.DeliveryDuration = "messaging.masstransit.delivery.duration";
            options.SendTotal = "messaging.masstransit.send";
            options.SendFaultTotal = "messaging.masstransit.send.errors";
            options.OutboxSendTotal = "messaging.masstransit.outbox.send";
            options.OutboxSendFaultTotal = "messaging.masstransit.outbox.send.errors";
            options.ActivityExecuteTotal = "messaging.masstransit.execute";
            options.ActivityExecuteFaultTotal = "messaging.masstransit.execute.errors";
            options.ActivityExecuteDuration = "messaging.masstransit.execute.duration";
            options.ExecuteInProgress = "messaging.masstransit.execute.active";
            options.ActivityCompensateTotal = "messaging.masstransit.compensate";
            options.ActivityCompensateFailureTotal = "messaging.masstransit.compensate.errors";
            options.ActivityCompensateDuration = "messaging.masstransit.compensate.duration";
            options.CompensateInProgress = "messaging.masstransit.compensate.active";
            options.BusInstances = "messaging.masstransit.bus";
            options.EndpointInstances = "messaging.masstransit.endpoint";
            options.HandlerInProgress = "messaging.masstransit.handler.active";
            options.SagaInProgress = "messaging.masstransit.saga.active";
        }
    }
}

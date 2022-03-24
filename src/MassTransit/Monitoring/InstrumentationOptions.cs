namespace MassTransit.Monitoring
{
    public class InstrumentationOptions
    {
        public string EndpointLabel { get; set; }
        public string ConsumerTypeLabel { get; set; }
        public string ExceptionTypeLabel { get; set; }
        public string MessageTypeLabel { get; set; }
        public string ActivityNameLabel { get; set; }
        public string ArgumentTypeLabel { get; set; }
        public string LogTypeLabel { get; set; }
        public string ServiceNameLabel { get; set; }

        public string ReceiveTotal { get; set; }
        public string ReceiveFaultTotal { get; set; }
        public string ReceiveDuration { get; set; }
        public string ReceiveInProgress { get; set; }

        public string ConsumeTotal { get; set; }
        public string ConsumeFaultTotal { get; set; }
        public string ConsumeRetryTotal { get; set; }
        public string PublishTotal { get; set; }
        public string PublishFaultTotal { get; set; }
        public string SendTotal { get; set; }
        public string SendFaultTotal { get; set; }
        public string ActivityExecuteTotal { get; set; }
        public string ActivityExecuteFaultTotal { get; set; }
        public string ActivityExecuteDuration { get; set; }
        public string ActivityCompensateTotal { get; set; }
        public string ActivityCompensateFailureTotal { get; set; }
        public string ActivityCompensateDuration { get; set; }

        public string BusInstances { get; set; }
        public string EndpointInstances { get; set; }
        public string ConsumerInProgress { get; set; }
        public string HandlerInProgress { get; set; }
        public string SagaInProgress { get; set; }
        public string ExecuteInProgress { get; set; }
        public string CompensateInProgress { get; set; }

        public string ConsumeDuration { get; set; }
        public string DeliveryDuration { get; set; }

        public static InstrumentationOptions CreateDefault()
        {
            return new InstrumentationOptions
            {
                EndpointLabel = "messaging.destination",
                ConsumerTypeLabel = "messaging.masstransit.consumer_type",
                ExceptionTypeLabel = "messaging.masstransit.exception_type",
                MessageTypeLabel = "messaging.masstransit.message_type",
                ActivityNameLabel = "messaging.masstransit.activity_type",
                ArgumentTypeLabel = "messaging.masstransit.argument_type",
                LogTypeLabel = "messaging.masstransit.log_type",
                ServiceNameLabel = "messaging.service",
                ReceiveTotal = "messaging.receive",
                ReceiveFaultTotal = "messaging.receive.errors",
                ReceiveDuration = "messaging.receive.duration",
                ReceiveInProgress = "messaging.receive.active",
                ConsumeTotal = "messaging.consume",
                ConsumeFaultTotal = "messaging.consume.errors",
                ConsumeRetryTotal = "messaging.consume.retries",
                ConsumeDuration = "messaging.consume.duration",
                ConsumerInProgress = "messaging.consume.active",
                DeliveryDuration = "messaging.delivery.duration",
                PublishTotal = "messaging.publish",
                PublishFaultTotal = "messaging.publish.errors",
                SendTotal = "messaging.send",
                SendFaultTotal = "messaging.send.errors",
                ActivityExecuteTotal = "messaging.masstransit.execute",
                ActivityExecuteFaultTotal = "messaging.masstransit.execute.errors",
                ActivityExecuteDuration = "messaging.masstransit.execute.duration",
                ExecuteInProgress = "messaging.masstransit.execute.active",
                ActivityCompensateTotal = "messaging.masstransit.compensate",
                ActivityCompensateFailureTotal = "messaging.masstransit.compensate.errors",
                ActivityCompensateDuration = "messaging.masstransit.compensate.duration",
                CompensateInProgress = "messaging.masstransit.compensate.active",
                BusInstances = "messaging.masstransit.bus",
                EndpointInstances = "messaging.masstransit.endpoint",
                HandlerInProgress = "messaging.masstransit.handler.active",
                SagaInProgress = "messaging.masstransit.saga.active",
            };
        }
    }
}

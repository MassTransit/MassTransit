namespace MassTransit.Monitoring
{
    using System;


    public class InstrumentationOptions
    {
        public const string MeterName = "MassTransit";

        public string ServiceName { get; set; }

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

        public string SagaTotal { get; set; }
        public string SagaFaultTotal { get; set; }
        public string SagaDuration { get; set; }

        public string HandlerTotal { get; set; }
        public string HandlerFaultTotal { get; set; }
        public string HandlerDuration { get; set; }

        [Obsolete]
        public string PublishTotal { get; set; }

        [Obsolete]
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

        public string OutboxSendTotal { get; set; }
        public string OutboxSendFaultTotal { get; set; }

        public string OutboxDeliveryTotal { get; set; }
        public string OutboxDeliveryFaultTotal { get; set; }
    }
}

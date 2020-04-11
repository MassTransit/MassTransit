namespace MassTransit.PrometheusIntegration
{
    public class PrometheusMetricsOptions
    {
        public string EndpointLabel { get; set; }
        public string ConsumerTypeLabel { get; set; }
        public string ExceptionTypeLabel { get; set; }
        public string MessageTypeLabel { get; set; }
        public string ServiceNameLabel { get; set; }

        public double[] HistogramBuckets { get; set; }

        public string ReceiveTotal { get; set; }
        public string ReceiveFaultTotal { get; set; }
        public string ReceiveDuration { get; set; }
        public string ReceiveInProgress { get; set; }

        public string MessageConsumeTotal { get; set; }
        public string MessageConsumeFaultTotal { get; set; }
        public string MessagePublishTotal { get; set; }
        public string MessagePublishFaultTotal { get; set; }
        public string MessageSendTotal { get; set; }
        public string MessageSendFaultTotal { get; set; }

        public string BusInstances { get; set; }
        public string EndpointInstances { get; set; }
        public string ConsumerInProgress { get; set; }
        public string HandlerInProgress { get; set; }
        public string SagaInProgress { get; set; }
        public string ActivityInProgress { get; set; }

        public string MessageConsumeDuration { get; set; }
        public string MessageDeliveryDuration { get; set; }

        public static PrometheusMetricsOptions CreateDefault() =>
            new PrometheusMetricsOptions
            {
                EndpointLabel = "endpoint_address",
                ConsumerTypeLabel = "consumer_type",
                ExceptionTypeLabel = "exception_type",
                MessageTypeLabel = "message_type",
                ServiceNameLabel = "service_name",
                HistogramBuckets = new[] {0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 30, 60, 120, 180, 240, 300},
                ReceiveTotal = "mt_receive_total",
                ReceiveFaultTotal = "mt_receive_fault_total",
                ReceiveDuration = "mt_receive_duration_seconds",
                ReceiveInProgress = "mt_receive_in_progress",
                MessageConsumeTotal = "mt_consume_total",
                MessageConsumeFaultTotal = "mt_consume_fault_total",
                MessageConsumeDuration = "mt_consume_duration_seconds",
                MessageDeliveryDuration = "mt_delivery_duration_seconds",
                MessagePublishTotal = "mt_publish_total",
                MessagePublishFaultTotal = "mt_publish_fault_total",
                MessageSendTotal = "mt_send_total",
                MessageSendFaultTotal = "mt_send_fault_total",
                BusInstances = "mt_bus",
                EndpointInstances = "mt_endpoint",
                ConsumerInProgress = "mt_consumer_in_progress",
                HandlerInProgress = "mt_handler_in_progress",
                SagaInProgress = "mt_saga_in_progress",
                ActivityInProgress = "mt_activity_in_progress",
            };
    }
}

namespace MassTransit
{
    public class PrometheusMetricsOptions
    {
        public string EndpointLabel { get; set; }
        public string ConsumerTypeLabel { get; set; }
        public string ExceptionTypeLabel { get; set; }
        public string MessageTypeLabel { get; set; }
        public string ActivityNameLabel { get; set; }
        public string ArgumentTypeLabel { get; set; }
        public string LogTypeLabel { get; set; }
        public string ServiceNameLabel { get; set; }

        public double[] HistogramBuckets { get; set; }

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

        public static PrometheusMetricsOptions CreateDefault()
        {
            return new PrometheusMetricsOptions
            {
                EndpointLabel = "endpoint_address",
                ConsumerTypeLabel = "consumer_type",
                ExceptionTypeLabel = "exception_type",
                MessageTypeLabel = "message_type",
                ActivityNameLabel = "activity_name",
                ArgumentTypeLabel = "argument_type",
                LogTypeLabel = "log_type",
                ServiceNameLabel = "service_name",
                HistogramBuckets = new[] {0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 30, 60, 120, 180, 240, 300},
                ReceiveTotal = "mt_receive_total",
                ReceiveFaultTotal = "mt_receive_fault_total",
                ReceiveDuration = "mt_receive_duration_seconds",
                ReceiveInProgress = "mt_receive_in_progress",
                ConsumeTotal = "mt_consume_total",
                ConsumeFaultTotal = "mt_consume_fault_total",
                ConsumeRetryTotal = "mt_consume_retry_total",
                ConsumeDuration = "mt_consume_duration_seconds",
                DeliveryDuration = "mt_delivery_duration_seconds",
                PublishTotal = "mt_publish_total",
                PublishFaultTotal = "mt_publish_fault_total",
                SendTotal = "mt_send_total",
                SendFaultTotal = "mt_send_fault_total",
                ActivityExecuteTotal = "mt_activity_execute_total",
                ActivityExecuteFaultTotal = "mt_activity_execute_fault_total",
                ActivityExecuteDuration = "mt_activity_execute_duration",
                ActivityCompensateTotal = "mt_activity_compensate_total",
                ActivityCompensateFailureTotal = "mt_activity_compensate_failure_total",
                ActivityCompensateDuration = "mt_activity_compensate_duration",
                BusInstances = "mt_bus",
                EndpointInstances = "mt_endpoint",
                ConsumerInProgress = "mt_consumer_in_progress",
                HandlerInProgress = "mt_handler_in_progress",
                SagaInProgress = "mt_saga_in_progress",
                ExecuteInProgress = "mt_activity_execute_in_progress",
                CompensateInProgress = "mt_activity_compensate_in_progress"
            };
        }
    }
}

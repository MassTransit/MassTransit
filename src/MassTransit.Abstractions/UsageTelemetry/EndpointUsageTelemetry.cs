namespace MassTransit.UsageTelemetry;

public class EndpointUsageTelemetry
{
    public string? Name { get; set; }
    public string? Type { get; set; }

    public int? PrefetchCount { get; set; }
    public int? ConcurrentMessageLimit { get; set; }

    public int HandlerCount { get; set; }
    public int ConsumerCount { get; set; }
    public int JobConsumerCount { get; set; }
    public int SagaCount { get; set; }
    public int SagaStateMachineCount { get; set; }
    public int ActivityCount { get; set; }
    public int ExecuteActivityCount { get; set; }
    public int CompensateActivityCount { get; set; }
    public int MessageCount { get; set; }
}

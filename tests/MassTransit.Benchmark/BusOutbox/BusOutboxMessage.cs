namespace MassTransitBenchmark.BusOutbox;

using System;


public record BusOutboxMessage(Guid CorrelationId, string Payload);

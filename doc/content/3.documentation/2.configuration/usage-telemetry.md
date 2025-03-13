# Usage Telemetry

As part of our ongoing commitment to improving MassTransit, in v8.40 we are introducing anonymous usage telemetry to better understand how the community utilizes the framework. This feature will provide valuable insights into bus configurations, transport usage, and endpoint patterns, helping us identify trends, optimize performance, and prioritize future enhancements. By collecting only non-identifiable data, we ensure user privacy while enabling more informed development decisions that benefit the entire MassTransit ecosystem.

We understand that not everyone may want to participate in usage telemetry, which is why we have made it easy to opt out. If you prefer not to send anonymous usage data, you can disable telemetry through a simple configuration setting. This ensures that no information is transmitted while still allowing you to use MassTransit without any impact on functionality. Our goal is to provide transparency and choice, so you can decide what works best for your environment.

::alert{type="info"}
The collected usage telemetry is strictly for internal use and is not shared, sold, or made publicly available. Its sole purpose is to help the MassTransit team analyze usage patterns, improve performance, and guide future development based on real-world adoption. No personally identifiable information is collected, ensuring that all data remains anonymous and focused solely on enhancing the framework for the community.
::

## Content

The example report below demonstrates the collected data. By default, only minimal details about the host, bus, and endpoints are included, along with basic configuration and counts for each consumer or message type.

```json
{
  "id": "bd740008-ebb8-e450-a00a-08dd60e7bc9c",
  "bus": [
    {
      "name": "IBus",
      "created": "2025-03-11T16:57:37.4587070-05:00",
      "started": "2025-03-11T16:57:37.9216500-05:00",
      "endpoints": [
        {
          "name": "odd-job",
          "type": "RabbitMQ",
          "consumer_count": 5,
          "prefetch_count": 64,
          "job_consumer_count": 1
        },
        {
          "name": "odd-job-completed",
          "type": "RabbitMQ",
          "consumer_count": 1,
          "prefetch_count": 1,
          "concurrent_message_limit": 1
        },
        {
          "name": "job-type",
          "type": "RabbitMQ",
          "prefetch_count": 64,
          "concurrent_message_limit": 16,
          "saga_state_machine_count": 1
        },
        {
          "name": "job",
          "type": "RabbitMQ",
          "prefetch_count": 64,
          "concurrent_message_limit": 16,
          "saga_state_machine_count": 1
        },
        {
          "name": "job-attempt",
          "type": "RabbitMQ",
          "prefetch_count": 64,
          "concurrent_message_limit": 16,
          "saga_state_machine_count": 1
        }
      ],
      "configured": "2025-03-11T16:57:37.8701060-05:00"
    }
  ],
  "host": {
    "time_zone_info": "(UTC-06:00) Central Time (Chicago)",
    "framework_version": "9.0.0",
    "mass_transit_version": "1.0.0.0",
    "operating_system_version": "Unix 14.7.2"
  },
  "created": "2025-03-12T16:57:37.4572470-05:00"
}
```

## Reporting

Usage telemetry is sent once, five minutes after the bus starts (or after all buses have started if you're using MultiBus). If the bus runs for less than five minutes, no data is reported. Telemetry is not captured or sent when using the test harness. Reporting is non-blocking and does not impact the startup or shutdown of MassTransit or your application. All reports are securely transmitted to an endpoint within the `masstransit.io` domain.

## Opt-Out

If you prefer not to send anonymous usage data, you can disable telemetry through a simple configuration setting. This ensures that no information is transmitted while still allowing you to use MassTransit without any impact on functionality.

#### Configuration 

To disable usage telemetry, specify `DisableUsageTelemetry` on the MassTransit configuration. When disabled, no usage telemetry is captured or reported.

```csharp
services.AddMassTransit(x =>
{
    x.DisableUsageTelemetry();
});
```

#### Environment Variable

You can also disable usage telemetry using an environment variable. Set:

`MASSTRANSIT_USAGE_TELEMETRY=false`

To disable usage telemetry. 

## Configuration 

To provide additional information, such as your _CustomerId_, usage telemetry can be configured as shown.

```csharp
services.AddMassTransit(x =>
{
    x.ConfigureUsageTelemetryOptions(options =>
    {
        options.CustomerId = "8675309";
    });
});
```



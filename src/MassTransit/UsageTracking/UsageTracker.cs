#nullable enable
namespace MassTransit.UsageTracking;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Internals;
using Logging;
using Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UsageTelemetry;


public class UsageTracker :
    IUsageTracker,
    IAsyncDisposable
{
    readonly TimeSpan _httpClientTimeout = TimeSpan.FromSeconds(10);
    readonly UsageTelemetryOptions _options;
    readonly List<IUsageTelemetrySource> _usages;
    CancellationTokenSource? _cancellationTokenSource;
    bool _disposed;
    ILogContext? _logContext;
    Task? _reportTask;

    public UsageTracker(IOptions<UsageTelemetryOptions> usageTrackingOptions)
    {
        _options = usageTrackingOptions.Value;
        _usages = [];

        var variable = Environment.GetEnvironmentVariable("MASSTRANSIT_USAGE_TELEMETRY");
        if (!string.IsNullOrWhiteSpace(variable))
        {
            if (bool.TryParse(variable, out var enabled))
                _options.Enabled = enabled;
            else if (int.TryParse(variable, out var value))
                _options.Enabled = value == 1;
            else
                LogContext.Warning?.Log("Unrecognized MASSTRANSIT_USAGE_TELEMETRY value: {Variable}. Should be true or false, 1 or 0", variable);
        }

        if (_options.Enabled)
        {
            Telemetry = new MassTransitUsageTelemetry
            {
                Id = NewId.NextGuid(),
                CustomerId = _options.CustomerId,
                Created = DateTimeOffset.Now.ToString("O"),
                Host = new HostUsageTelemetry
                {
                    MassTransitVersion = HostMetadataCache.Host.MassTransitVersion,
                    FrameworkVersion = HostMetadataCache.Host.FrameworkVersion,
                    OperatingSystemVersion = HostMetadataCache.Host.OperatingSystemVersion,
                    TimeZoneInfo = TimeZoneInfo.Local.ToString(),
                }
            };
        }
    }

    [MemberNotNullWhen(true, nameof(Enabled))]
    MassTransitUsageTelemetry? Telemetry { get; }

    public bool Enabled => _options.Enabled;

    public async ValueTask DisposeAsync()
    {
        _disposed = true;

        if (_reportTask is { IsCompleted: true })
            return;

        var cancellationTokenSource = _cancellationTokenSource;
        if (cancellationTokenSource == null)
            return;

        if (!cancellationTokenSource.IsCancellationRequested)
            cancellationTokenSource.Cancel();
        try
        {
            if (_reportTask != null)
                await _reportTask.OrTimeout(_httpClientTimeout).ConfigureAwait(false);
        }
        catch (Exception)
        {
            //
        }
    }

    public void PreConfigureBus<T>(T configurator, IBusRegistrationContext context)
        where T : IBusFactoryConfigurator
    {
        if (!_options.Enabled || Telemetry is null)
            return;

        _logContext ??= LogContext.Current;

        var busName = nameof(IBus);

        var selector = context.GetService<IContainerSelector>();
        if (selector != null && selector.GetType().ClosesType(typeof(DependencyInjectionContainerRegistrar<>), out Type[] types))
            busName = types[0].Name;

        var busUsageTelemetry = new BusUsageTelemetry
        {
            Name = busName,
            Created = DateTimeOffset.Now.ToString("O"),
            Endpoints = []
        };
        Telemetry.Bus ??= [];
        Telemetry.Bus.Add(busUsageTelemetry);

        var observer = new UsageTelemetryBusObserver(this, busUsageTelemetry);
        configurator.ConnectBusObserver(observer);

        var endpointConfigurationObserver = new UsageTelemetryEndpointConfigurationObserver(busUsageTelemetry, Telemetry);

        _ = configurator.ConnectEndpointConfigurationObserver(endpointConfigurationObserver);

        _usages.Add(endpointConfigurationObserver);
    }

    public void PreConfigureRider<T>(T configurator)
        where T : IRiderFactoryConfigurator
    {
        if (!_options.Enabled || Telemetry is null)
            return;

        _logContext ??= LogContext.Current;

        var riderType = configurator.GetType().Name switch
        {
            "EventHubFactoryConfigurator" => "EventHub",
            "KafkaFactoryConfigurator" => "Kafka",
            _ => configurator.GetType().Name
        };

        Telemetry.Rider ??= [];
        var riderUsage = Telemetry.Rider.LastOrDefault(x => x.RiderType == riderType);
        if (riderUsage == null)
        {
            riderUsage = new RiderUsageTelemetry
            {
                RiderType = riderType,
                Endpoints = []
            };
            Telemetry.Rider.Add(riderUsage);
        }
    }

    public void PostCreateBus(IBus bus, BusUsageTelemetry busTelemetry)
    {
        if (!_options.Enabled)
            return;

        _logContext ??= LogContext.Current;

        busTelemetry.Configured = DateTimeOffset.Now.ToString("O");
    }

    public void PostStartBus(IBus bus, BusUsageTelemetry busTelemetry)
    {
        if (!_options.Enabled || Telemetry is null)
            return;

        _logContext ??= LogContext.Current;

        busTelemetry.Started = DateTimeOffset.Now.ToString("O");

        if (Telemetry.Bus?.Any(x => x.Started is null) ?? true)
            return;

        _usages.ForEach(x => x.Update());

        if (_disposed)
            return;

        _reportTask = Task.Run(() => ReportUsageTelemetry());
    }

    async Task ReportUsageTelemetry()
    {
        if (Telemetry is null)
            return;

        if (_disposed)
            return;

        if (_logContext is not null)
            LogContext.SetCurrentIfNull(_logContext);

        try
        {
            Interlocked.CompareExchange(ref _cancellationTokenSource, new CancellationTokenSource(), null);

            var delay = _options.ReportDelay ?? TimeSpan.FromMinutes(5);

            var maxDelay = TimeSpan.FromMinutes(30);
            if (delay > maxDelay)
                delay = maxDelay;

            try
            {
                await Task.Delay(delay, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }

            if (_cancellationTokenSource.IsCancellationRequested && _options.ReportOnShutdown == false)
                return;

            var json = JsonSerializer.Serialize(Telemetry!, UsageTelemetrySerializerContext.Default.MassTransitUsageTelemetry);

            LogContext.Info?.Log("Usage Telemetry: {Telemetry}", json);

            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            httpClient.Timeout = _httpClientTimeout;

            var uri = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttps,
                Host = string.Join(".", "usage-tracking", "masstransit", "io"),
                Path = "/usage",
            }.Uri;

            var response = await httpClient.PostAsync(uri, jsonContent);
            if (response.StatusCode != HttpStatusCode.Accepted)
                LogContext.Warning?.Log("Failed to report usage telemetry: {StatusCode}", response.StatusCode);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            LogContext.Warning?.Log(exception, "Failed to report usage telemetry");
        }
    }
}

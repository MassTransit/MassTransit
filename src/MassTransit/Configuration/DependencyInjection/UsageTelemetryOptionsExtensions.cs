#nullable enable
namespace MassTransit;

using System;
using Microsoft.Extensions.DependencyInjection;


public static class UsageTelemetryOptionsExtensions
{
    /// <summary>
    /// Configure the usage telemetry options
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IBusRegistrationConfigurator ConfigureUsageTelemetryOptions(this IBusRegistrationConfigurator configurator,
        Action<UsageTelemetryOptions>? callback)
    {
        configurator.AddOptions<UsageTelemetryOptions>()
            .Configure(options =>
            {
                callback?.Invoke(options);
            });

        return configurator;
    }

    /// <summary>
    /// Configure the usage telemetry options
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IBusRegistrationConfigurator<T> ConfigureUsageTelemetryOptions<T>(this IBusRegistrationConfigurator<T> configurator,
        Action<UsageTelemetryOptions>? callback)
        where T : class, IBus
    {
        configurator.AddOptions<UsageTelemetryOptions>()
            .Configure(options =>
            {
                callback?.Invoke(options);
            });

        return configurator;
    }

    /// <summary>
    /// Disable the collection and reporting of usage telemetry
    /// </summary>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static IBusRegistrationConfigurator DisableUsageTelemetry(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddOptions<UsageTelemetryOptions>()
            .Configure(options => options.Enabled = false);

        return configurator;
    }
}

namespace MassTransit.SignalR
{
    using System;
    using Microsoft.AspNetCore.SignalR;


    public static class MassTransitDependencyInjectionExtensions
    {
        [Obsolete("Do not use this method anymore.")]
        /// <summary>
        /// Registers the Hub Lifetime Manager based on the configuration options provided
        /// </summary>
        /// <param name="signalRServerBuilder">The SignalR builder abstraction for configuring SignalR servers.</param>
        /// <param name="configureOptions">The MassTransit SignalR configuration options</param>
        public static ISignalRServerBuilder AddMassTransitBackplane(this ISignalRServerBuilder signalRServerBuilder,
            Action<MassTransitSignalROptions> configureOptions = null)
        {
            return signalRServerBuilder;
        }
    }
}

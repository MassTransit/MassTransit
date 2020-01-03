namespace MassTransit.Logging
{
    using System.Collections.Generic;
    using Context;
    using Metadata;


    public static class ConsumerDiagnosticSourceExtensions
    {
        public static StartedActivity? StartConsumerActivity<TConsumer, T>(this EnabledDiagnosticSource source)
            where T : class
        {
            return source.StartActivity(null, GetConsumerTags<T>(TypeMetadataCache<TConsumer>.ShortName));
        }

        public static StartedActivity? StartHandlerActivity<T>(this EnabledDiagnosticSource source)
            where T : class
        {
            return source.StartActivity(null, GetConsumerTags<T>());
        }

        static IEnumerable<(string, string)> GetConsumerTags<T>(string consumerType = null)
            where T : class
        {
            yield return (DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            yield return (DiagnosticHeaders.PeerHost, HostMetadataCache.Host.MachineName);
            yield return (DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);
            yield return (DiagnosticHeaders.PeerService, "Consumer");

            if (consumerType != null)
                yield return (DiagnosticHeaders.ConsumerType, consumerType);
        }
    }
}

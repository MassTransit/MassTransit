namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Logging;
    using Metadata;


    public static class SagaDiagnosticSourceExtensions
    {
        public static StartedActivity? StartSagaActivity<TSaga, T>(this EnabledDiagnosticSource source)
            where TSaga : ISaga
            where T : class
        {
            return source.StartActivity(null, GetSagaTags<TSaga, T>());
        }

        public static StartedActivity? StartSagaActivity<TSaga, T>(this EnabledDiagnosticSource source, Guid sagaId, string beginState = null)
            where TSaga : ISaga
            where T : class
        {
            return source.StartActivity(null, GetSagaTags<TSaga, T>(sagaId, beginState));
        }

        static IEnumerable<(string, string)> GetSagaTags<TSaga, T>()
        {
            yield return (DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            yield return (DiagnosticHeaders.PeerHost, HostMetadataCache.Host.MachineName);
            yield return (DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);
            yield return (DiagnosticHeaders.PeerService, "Repository");

            yield return (DiagnosticHeaders.SagaType, TypeMetadataCache<TSaga>.ShortName);
        }

        static IEnumerable<(string, string)> GetSagaTags<TSaga, T>(Guid sagaId, string beginState)
        {
            yield return (DiagnosticHeaders.ServiceKind, DiagnosticHeaders.Kind.Consumer);
            yield return (DiagnosticHeaders.PeerHost, HostMetadataCache.Host.MachineName);
            yield return (DiagnosticHeaders.PeerAddress, TypeMetadataCache<T>.DiagnosticAddress);
            yield return (DiagnosticHeaders.PeerService, "Saga");

            yield return (DiagnosticHeaders.SagaType, TypeMetadataCache<TSaga>.ShortName);

            yield return (DiagnosticHeaders.SagaId, sagaId.ToString("D"));

            if (beginState != null)
                yield return (DiagnosticHeaders.BeginState, beginState);
        }
    }
}

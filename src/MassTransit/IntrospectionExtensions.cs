namespace MassTransit
{
    using System.Threading;
    using Introspection;


    public static class IntrospectionExtensions
    {
        public static ProbeResult GetProbeResult(this IProbeSite probeSite, CancellationToken cancellationToken = default)
        {
            var builder = new ProbeResultBuilder(NewId.NextGuid(), cancellationToken);

            probeSite.Probe(builder);

            return builder.Build();
        }
    }
}

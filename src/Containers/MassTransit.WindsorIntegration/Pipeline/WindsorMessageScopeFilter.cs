namespace MassTransit.WindsorIntegration.Pipeline
{
    using System.Threading.Tasks;
    using Castle.MicroKernel.Lifestyle.Scoped;
    using GreenPipes;


    /// <summary>
    /// Calls by the inbound message pipeline to begin and end a message scope
    /// in the container.
    /// </summary>
    public class WindsorMessageScopeFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("windsorMessageScope");
        }

        async Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            if (CallContextLifetimeScope.ObtainCurrentScope() is MessageLifetimeScope)
                await next.Send(context).ConfigureAwait(false);
            else
                using (new MessageLifetimeScope(context))
                {
                    await next.Send(context).ConfigureAwait(false);
                }
        }
    }
}

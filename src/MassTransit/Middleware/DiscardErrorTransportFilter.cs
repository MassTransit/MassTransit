namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Discard the error instead of moving it to the error transport.
    /// </summary>
    public class DiscardErrorTransportFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("discardFault");
        }

        Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            return next.Send(context);
        }
    }
}

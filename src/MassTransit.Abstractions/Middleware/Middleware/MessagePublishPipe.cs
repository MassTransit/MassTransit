namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Converts an inbound context type to a pipe context type post-dispatch
    /// </summary>
    /// <typeparam name="TMessage">The subsequent pipe context type</typeparam>
    public class MessagePublishPipe<TMessage> :
        IMessagePublishPipe<TMessage>
        where TMessage : class
    {
        readonly IPipe<PublishContext<TMessage>> _outputPipe;

        public MessagePublishPipe(IPipe<PublishContext<TMessage>> outputPipe)
        {
            _outputPipe = outputPipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("messagePublishPipe");
            scope.Add("messageType", TypeCache<TMessage>.ShortName);

            _outputPipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        Task IPipe<PublishContext<TMessage>>.Send(PublishContext<TMessage> context)
        {
            return _outputPipe.Send(context);
        }
    }
}

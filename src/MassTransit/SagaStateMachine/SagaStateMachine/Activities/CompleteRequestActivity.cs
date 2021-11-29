namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Components;
    using Contracts;


    public class CompleteRequestActivity :
        IStateMachineActivity<RequestState, RequestCompleted>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("completeRequest");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<RequestState, RequestCompleted> context, IBehavior<RequestState, RequestCompleted> next)
        {
            if (!context.Saga.ExpirationTime.HasValue || context.Saga.ExpirationTime.Value > DateTime.UtcNow)
            {
                IPipe<SendContext> pipe = new RequestStateMessagePipe(context, context.Message.Payload, context.Message.PayloadType);

                var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress).ConfigureAwait(false);

                var dummyMessage = new CompletedEvent();

                await endpoint.Send(dummyMessage, pipe).ConfigureAwait(false);
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<RequestState, RequestCompleted, TException> context,
            IBehavior<RequestState, RequestCompleted> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }


        class CompletedEvent
        {
        }
    }
}

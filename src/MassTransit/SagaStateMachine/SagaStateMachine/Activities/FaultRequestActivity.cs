namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Components;
    using Contracts;


    public class FaultRequestActivity :
        IStateMachineActivity<RequestState, RequestFaulted>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("faultRequest");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<RequestState, RequestFaulted> context, IBehavior<RequestState, RequestFaulted> next)
        {
            if (!context.Saga.ExpirationTime.HasValue || context.Saga.ExpirationTime.Value > DateTime.UtcNow)
            {
                IPipe<SendContext> pipe = new RequestStateMessagePipe(context, context.Message.Payload, context.Message.PayloadType);

                var endpoint = await context.GetSendEndpoint(context.Saga.ResponseAddress).ConfigureAwait(false);

                var dummyMessage = new FaultedEvent();

                await endpoint.Send(dummyMessage, pipe).ConfigureAwait(false);
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<RequestState, RequestFaulted, TException> context,
            IBehavior<RequestState, RequestFaulted> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }


        class FaultedEvent
        {
        }
    }
}

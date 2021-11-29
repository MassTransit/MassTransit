namespace MassTransit.TestFramework.Sagas
{
    using System;
    using System.Threading.Tasks;


    public class PublishTestStartedActivity :
        IStateMachineActivity<TestInstance>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public PublishTestStartedActivity(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("publisher");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TestInstance> context, IBehavior<TestInstance> next)
        {
            await context.Publish(new TestStarted
            {
                CorrelationId = context.Saga.CorrelationId,
                TestKey = context.Saga.Key
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TestInstance, T> context, IBehavior<TestInstance, T> next)
            where T : class
        {
            await context.Publish(new TestStarted
            {
                CorrelationId = context.Saga.CorrelationId,
                TestKey = context.Saga.Key
            });

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TestInstance, TException> context, IBehavior<TestInstance> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TestInstance, T, TException> context, IBehavior<TestInstance, T> next)
            where TException : Exception
            where T : class
        {
            return next.Faulted(context);
        }
    }
}

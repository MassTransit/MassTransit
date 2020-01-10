namespace MassTransit.TestFramework.Sagas
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;


    public class PublishTestStartedActivity :
        Activity<TestInstance>
    {
        readonly ConsumeContext _context;

        public PublishTestStartedActivity(ConsumeContext context)
        {
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("publisher");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TestInstance> context, Behavior<TestInstance> next)
        {
            await _context.Publish(new TestStarted {CorrelationId = context.Instance.CorrelationId, TestKey = context.Instance.Key}).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Execute<T>(BehaviorContext<TestInstance, T> context, Behavior<TestInstance, T> next)
        {
            await _context.Publish(new TestStarted {CorrelationId = context.Instance.CorrelationId, TestKey = context.Instance.Key}).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TestInstance, TException> context, Behavior<TestInstance> next) where TException : Exception
        {
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TestInstance, T, TException> context, Behavior<TestInstance, T> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
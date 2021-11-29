namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class ActivityBehavior<TSaga> :
        IBehavior<TSaga>
        where TSaga : class, ISaga
    {
        readonly IStateMachineActivity<TSaga> _activity;
        readonly IBehavior<TSaga> _next;

        public ActivityBehavior(IStateMachineActivity<TSaga> activity, IBehavior<TSaga> next)
        {
            _activity = activity;
            _next = next;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this, x =>
            {
                _activity.Accept(visitor);
                _next.Accept(visitor);
            });
        }

        public void Probe(ProbeContext context)
        {
            _activity.Probe(context);
            _next.Probe(context);
        }

        public async Task Execute(BehaviorContext<TSaga> context)
        {
            try
            {
                await _activity.Execute(context, _next).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionTypeCache.Faulted(_next, context, exception).ConfigureAwait(false);
            }
        }

        public async Task Execute<T>(BehaviorContext<TSaga, T> context)
            where T : class
        {
            var behavior = new DataBehavior<TSaga, T>(_next);
            try
            {
                await _activity.Execute(context, behavior).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await ExceptionTypeCache.Faulted(behavior, context, exception).ConfigureAwait(false);
            }
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<TSaga, T, TException> context)
            where T : class
            where TException : Exception
        {
            var behavior = new DataBehavior<TSaga, T>(_next);

            return _activity.Faulted(context, behavior);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TException> context)
            where TException : Exception
        {
            return _activity.Faulted(context, _next);
        }
    }
}

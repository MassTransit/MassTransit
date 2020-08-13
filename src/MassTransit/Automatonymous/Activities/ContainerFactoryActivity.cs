namespace Automatonymous.Activities
{
    using System.Threading.Tasks;
    using Behaviors;
    using GreenPipes;


    public class ContainerFactoryActivity<TInstance, TActivity> :
        Activity<TInstance>
        where TActivity : class, Activity<TInstance>
    {
        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        Task Activity<TInstance>.Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            var factory = context.GetStateMachineActivityFactory();

            Activity<TInstance> activity = factory.GetActivity<TActivity, TInstance>(context);

            return activity.Execute(context, next);
        }

        Task Activity<TInstance>.Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            var factory = context.GetStateMachineActivityFactory();

            Activity<TInstance> activity = factory.GetActivity<TActivity, TInstance>(context);

            var widenBehavior = new WidenBehavior<TInstance, T>(next, context);

            return activity.Execute(context, widenBehavior);
        }

        Task Activity<TInstance>.Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next)
        {
            return next.Faulted(context);
        }

        Task Activity<TInstance>.Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next)
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }
    }


    public class ContainerFactoryActivity<TInstance, TData, TActivity> :
        Activity<TInstance, TData>
        where TActivity : class, Activity<TInstance, TData>
    {
        void Visitable.Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateScope("containerActivityFactory");
        }

        Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            var factory = context.GetStateMachineActivityFactory();

            Activity<TInstance, TData> activity = factory.GetActivity<TActivity, TInstance, TData>(context);

            return activity.Execute(context, next);
        }

        Task Activity<TInstance, TData>.Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
        {
            return next.Faulted(context);
        }
    }
}

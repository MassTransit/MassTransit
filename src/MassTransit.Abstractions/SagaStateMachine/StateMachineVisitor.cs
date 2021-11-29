namespace MassTransit
{
    using System;


    public interface StateMachineVisitor
    {
        void Visit(State state, Action<State> next);

        void Visit(Event @event, Action<Event> next);

        void Visit<TMessage>(Event<TMessage> @event, Action<Event<TMessage>> next)
            where TMessage : class;

        void Visit(IStateMachineActivity activity);

        void Visit<T>(IBehavior<T> behavior)
            where T : class, ISaga;

        void Visit<T>(IBehavior<T> behavior, Action<IBehavior<T>> next)
            where T : class, ISaga;

        void Visit<T, TMessage>(IBehavior<T, TMessage> behavior)
            where T : class, ISaga
            where TMessage : class;

        void Visit<T, TMessage>(IBehavior<T, TMessage> behavior, Action<IBehavior<T, TMessage>> next)
            where T : class, ISaga
            where TMessage : class;

        void Visit(IStateMachineActivity activity, Action<IStateMachineActivity> next);
    }
}

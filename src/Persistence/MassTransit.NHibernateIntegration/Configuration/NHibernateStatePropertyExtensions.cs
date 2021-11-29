namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using NHibernate.Mapping.ByCode;
    using NHibernateIntegration;


    public static class NHibernateStatePropertyExtensions
    {
        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper, Expression<Func<T, State>> stateExpression)
            where T : class
            where TMachine : StateMachine, new()
        {
            StateMachineStateUserType<TMachine>.SaveAsString(new TMachine());

            mapper.Property(stateExpression, x =>
            {
                x.Type<StateMachineStateUserType<TMachine>>();
                x.NotNullable(true);
                x.Length(80);
            });
        }

        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper, Expression<Func<T, State>> stateExpression, TMachine machine)
            where T : class
            where TMachine : StateMachine, new()
        {
            StateMachineStateUserType<TMachine>.SaveAsString(machine);

            mapper.Property(stateExpression, x =>
            {
                x.Type<StateMachineStateUserType<TMachine>>();
                x.NotNullable(true);
                x.Length(80);
            });
        }

        public static void StateProperty<T, TMachine>(this IClassMapper<T> mapper, Expression<Func<T, State>> stateExpression, TMachine machine,
            State[] statesInOrder)
            where T : class
            where TMachine : StateMachine, new()
        {
            StateMachineStateUserType<TMachine>.SaveAsInt32(machine, statesInOrder);

            mapper.Property(stateExpression, x =>
            {
                x.Type<StateMachineStateUserType<TMachine>>();
                x.NotNullable(true);
                x.Length(80);
            });
        }

        public static void CompositeEventProperty<T>(this IClassMapper<T> mapper, Expression<Func<T, CompositeEventStatus>> compositeEventStatusExpression)
            where T : class
        {
            mapper.Property(compositeEventStatusExpression, x =>
            {
                x.Type<CompositeEventStatusUserType>();
                x.NotNullable(true);
            });
        }
    }
}

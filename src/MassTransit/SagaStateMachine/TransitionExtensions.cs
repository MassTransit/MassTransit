namespace MassTransit
{
    using System;
    using SagaStateMachine;


    public static class TransitionExtensions
    {
        /// <summary>
        /// Transition the state machine to the specified state
        /// </summary>
        public static EventActivityBinder<TSaga> TransitionTo<TSaga>(this EventActivityBinder<TSaga> source, State toState)
            where TSaga : class, ISaga
        {
            State<TSaga> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the specified state in response to an exception
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="source"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TSaga, TException> TransitionTo<TSaga, TException>(this ExceptionActivityBinder<TSaga, TException> source,
            State toState)
            where TSaga : class, ISaga
            where TException : Exception
        {
            State<TSaga> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            var compensateActivity = new ExecuteOnFaultedActivity<TSaga>(activity);

            return source.Add(compensateActivity);
        }

        /// <summary>
        /// Transition the state machine to the specified state
        /// </summary>
        public static EventActivityBinder<TSaga, TMessage> TransitionTo<TSaga, TMessage>(this EventActivityBinder<TSaga, TMessage> source, State toState)
            where TSaga : class, ISaga
            where TMessage : class
        {
            State<TSaga> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the specified state in response to an exception
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="source"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TSaga, TMessage, TException> TransitionTo<TSaga, TMessage, TException>(
            this ExceptionActivityBinder<TSaga, TMessage, TException> source, State toState)
            where TSaga : class, ISaga
            where TException : Exception
            where TMessage : class
        {
            State<TSaga> state = source.StateMachine.GetState(toState.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            var compensateActivity = new ExecuteOnFaultedActivity<TSaga>(activity);

            return source.Add(compensateActivity);
        }

        /// <summary>
        /// Transition the state machine to the Final state
        /// </summary>
        public static EventActivityBinder<TSaga, TMessage> Finalize<TSaga, TMessage>(this EventActivityBinder<TSaga, TMessage> source)
            where TSaga : class, ISaga
            where TMessage : class
        {
            State<TSaga> state = source.StateMachine.GetState(source.StateMachine.Final.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the Final state
        /// </summary>
        public static EventActivityBinder<TSaga> Finalize<TSaga>(this EventActivityBinder<TSaga> source)
            where TSaga : class, ISaga
        {
            State<TSaga> state = source.StateMachine.GetState(source.StateMachine.Final.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            return source.Add(activity);
        }

        /// <summary>
        /// Transition the state machine to the Final state
        /// </summary>
        public static ExceptionActivityBinder<TSaga, TMessage, TException> Finalize<TSaga, TMessage, TException>(
            this ExceptionActivityBinder<TSaga, TMessage, TException> source)
            where TSaga : class, ISaga
            where TException : Exception
            where TMessage : class
        {
            State<TSaga> state = source.StateMachine.GetState(source.StateMachine.Final.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            var compensateActivity = new ExecuteOnFaultedActivity<TSaga>(activity);

            return source.Add(compensateActivity);
        }

        /// <summary>
        /// Transition the state machine to the Final state
        /// </summary>
        public static ExceptionActivityBinder<TSaga, TException> Finalize<TSaga, TException>(this ExceptionActivityBinder<TSaga, TException> source)
            where TSaga : class, ISaga
            where TException : Exception
        {
            State<TSaga> state = source.StateMachine.GetState(source.StateMachine.Final.Name);

            var activity = new TransitionActivity<TSaga>(state, source.StateMachine.Accessor);

            var compensateActivity = new ExecuteOnFaultedActivity<TSaga>(activity);

            return source.Add(compensateActivity);
        }
    }
}

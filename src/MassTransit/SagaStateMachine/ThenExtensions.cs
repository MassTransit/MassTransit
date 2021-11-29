namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class ThenExtensions
    {
        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static EventActivityBinder<TSaga> Then<TSaga>(this EventActivityBinder<TSaga> binder, Action<BehaviorContext<TSaga>> action)
            where TSaga : class, ISaga
        {
            return binder.Add(new ActionActivity<TSaga>(action));
        }

        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static ExceptionActivityBinder<TSaga, TException> Then<TSaga, TException>(this ExceptionActivityBinder<TSaga, TException> binder,
            Action<BehaviorExceptionContext<TSaga, TException>> action)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return binder.Add(new FaultedActionActivity<TSaga, TException>(action));
        }

        /// <summary>
        /// Adds a asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="asyncAction">The asynchronous delegate</param>
        public static ExceptionActivityBinder<TSaga, TException> ThenAsync<TSaga, TException>(this ExceptionActivityBinder<TSaga, TException> binder,
            Func<BehaviorExceptionContext<TSaga, TException>, Task> asyncAction)
            where TSaga : class, ISaga
            where TException : Exception
        {
            return binder.Add(new AsyncFaultedActionActivity<TSaga, TException>(asyncAction));
        }

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The asynchronous delegate</param>
        public static EventActivityBinder<TSaga> ThenAsync<TSaga>(this EventActivityBinder<TSaga> binder, Func<BehaviorContext<TSaga>, Task> action)
            where TSaga : class, ISaga
        {
            return binder.Add(new AsyncActivity<TSaga>(action));
        }

        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static EventActivityBinder<TSaga, TData> Then<TSaga, TData>(this EventActivityBinder<TSaga, TData> binder,
            Action<BehaviorContext<TSaga, TData>> action)
            where TSaga : class, ISaga
            where TData : class
        {
            return binder.Add(new ActionActivity<TSaga, TData>(action));
        }

        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static ExceptionActivityBinder<TSaga, TData, TException> Then<TSaga, TData, TException>(
            this ExceptionActivityBinder<TSaga, TData, TException> binder,
            Action<BehaviorExceptionContext<TSaga, TData, TException>> action)
            where TSaga : class, ISaga
            where TException : Exception
            where TData : class
        {
            return binder.Add(new FaultedActionActivity<TSaga, TData, TException>(action));
        }

        /// <summary>
        /// Adds a asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="asyncAction">The asynchronous delegate</param>
        public static ExceptionActivityBinder<TSaga, TData, TException> ThenAsync<TSaga, TData, TException>(
            this ExceptionActivityBinder<TSaga, TData, TException> binder,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task> asyncAction)
            where TSaga : class, ISaga
            where TException : Exception
            where TData : class
        {
            return binder.Add(new AsyncFaultedActionActivity<TSaga, TData, TException>(asyncAction));
        }

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The asynchronous delegate</param>
        public static EventActivityBinder<TSaga, TData> ThenAsync<TSaga, TData>(this EventActivityBinder<TSaga, TData> binder,
            Func<BehaviorContext<TSaga, TData>, Task> action)
            where TSaga : class, ISaga
            where TData : class
        {
            return binder.Add(new AsyncActivity<TSaga, TData>(action));
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TSaga> Execute<TSaga>(this EventActivityBinder<TSaga> binder,
            Func<BehaviorContext<TSaga>, IStateMachineActivity<TSaga>> activityFactory)
            where TSaga : class, ISaga
        {
            var activity = new FactoryActivity<TSaga>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activity">An existing activity</param>
        public static EventActivityBinder<TSaga> Execute<TSaga>(this EventActivityBinder<TSaga> binder, IStateMachineActivity<TSaga> activity)
            where TSaga : class, ISaga
        {
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TSaga> ExecuteAsync<TSaga>(this EventActivityBinder<TSaga> binder,
            Func<BehaviorContext<TSaga>, Task<IStateMachineActivity<TSaga>>> activityFactory)
            where TSaga : class, ISaga
        {
            var activity = new AsyncFactoryActivity<TSaga>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TSaga, TData> Execute<TSaga, TData>(this EventActivityBinder<TSaga, TData> binder,
            Func<BehaviorContext<TSaga, TData>, IStateMachineActivity<TSaga, TData>> activityFactory)
            where TSaga : class, ISaga
            where TData : class
        {
            var activity = new FactoryActivity<TSaga, TData>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TSaga, TData> ExecuteAsync<TSaga, TData>(this EventActivityBinder<TSaga, TData> binder,
            Func<BehaviorContext<TSaga, TData>, Task<IStateMachineActivity<TSaga, TData>>> activityFactory)
            where TSaga : class, ISaga
            where TData : class
        {
            var activity = new AsyncFactoryActivity<TSaga, TData>(activityFactory);
            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TSaga, TData> Execute<TSaga, TData>(this EventActivityBinder<TSaga, TData> binder,
            Func<BehaviorContext<TSaga, TData>, IStateMachineActivity<TSaga>> activityFactory)
            where TSaga : class, ISaga
            where TData : class
        {
            var activity = new FactoryActivity<TSaga, TData>(context =>
            {
                IStateMachineActivity<TSaga> newActivity = activityFactory(context);

                return new SlimActivity<TSaga, TData>(newActivity);
            });

            return binder.Add(activity);
        }

        /// <summary>
        /// Add an activity execution to the event's behavior
        /// </summary>
        /// <typeparam name="TSaga">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="activityFactory">The factory method which returns the activity to execute</param>
        public static EventActivityBinder<TSaga, TData> ExecuteAsync<TSaga, TData>(this EventActivityBinder<TSaga, TData> binder,
            Func<BehaviorContext<TSaga, TData>, Task<IStateMachineActivity<TSaga>>> activityFactory)
            where TSaga : class, ISaga
            where TData : class
        {
            var activity = new AsyncFactoryActivity<TSaga, TData>(async context =>
            {
                IStateMachineActivity<TSaga> newActivity = await activityFactory(context).ConfigureAwait(false);

                return new SlimActivity<TSaga, TData>(newActivity);
            });

            return binder.Add(activity);
        }
    }
}

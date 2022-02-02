namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;


    public class CatchExceptionActivityBinder<TInstance, TException> :
        ExceptionActivityBinder<TInstance, TException>
        where TInstance : class, ISaga
        where TException : Exception
    {
        readonly IActivityBinder<TInstance>[] _activities;
        readonly StateMachine<TInstance> _machine;

        public Event Event { get; }

        public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event @event)
        {
            _activities = Array.Empty<IActivityBinder<TInstance>>();
            _machine = machine;
            Event = @event;
        }

        CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event @event,
            IActivityBinder<TInstance>[] activities,
            params IActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new IActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            Event = @event;
        }

        public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        public StateMachine<TInstance> StateMachine => _machine;

        public ExceptionActivityBinder<TInstance, TException> Add(IStateMachineActivity<TInstance> activity)
        {
            IActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(Event, activity);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, Event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
            where T : Exception
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, Event);

            binder = activityCallback(binder);

            IActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(Event, binder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, Event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> If(StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TException> IfAsync(StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TException> IfElse(StateMachineExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback)
        {
            ExceptionActivityBinder<TInstance, TException> thenBinder = GetBinder(thenActivityCallback);
            ExceptionActivityBinder<TInstance, TException> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TException>(Event, condition, thenBinder, elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, Event, _activities, conditionBinder);
        }

        public ExceptionActivityBinder<TInstance, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TInstance, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> elseActivityCallback)
        {
            ExceptionActivityBinder<TInstance, TException> thenBinder = GetBinder(thenActivityCallback);
            ExceptionActivityBinder<TInstance, TException> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TException>(Event, condition, thenBinder, elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TException>(_machine, Event, _activities, conditionBinder);
        }

        ExceptionActivityBinder<TInstance, TException> GetBinder(
            Func<ExceptionActivityBinder<TInstance, TException>, ExceptionActivityBinder<TInstance, TException>> callback)
        {
            ExceptionActivityBinder<TInstance, TException> thenBinder = new CatchExceptionActivityBinder<TInstance, TException>(_machine,
                Event);
            return callback(thenBinder);
        }
    }


    public class CatchExceptionActivityBinder<TInstance, TData, TException> :
        ExceptionActivityBinder<TInstance, TData, TException>
        where TInstance : class, ISaga
        where TException : Exception
        where TData : class
    {
        readonly IActivityBinder<TInstance>[] _activities;
        readonly StateMachine<TInstance> _machine;

        public Event<TData> Event { get; }

        public CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> @event)
        {
            _activities = Array.Empty<IActivityBinder<TInstance>>();
            _machine = machine;
            Event = @event;
        }

        CatchExceptionActivityBinder(StateMachine<TInstance> machine, Event<TData> @event,
            IActivityBinder<TInstance>[] activities,
            params IActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new IActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _machine = machine;
            Event = @event;
        }

        public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
        {
            return _activities;
        }

        public StateMachine<TInstance> StateMachine => _machine;

        public ExceptionActivityBinder<TInstance, TData, TException> Add(IStateMachineActivity<TInstance> activity)
        {
            IActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(Event, activity);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, Event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Add(IStateMachineActivity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            IActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(Event, converterActivity);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, Event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
            where T : Exception
        {
            ExceptionActivityBinder<TInstance, TData, T> binder = new CatchExceptionActivityBinder<TInstance, TData, T>(_machine, Event);

            binder = activityCallback(binder);

            IActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(Event, binder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, Event, _activities, activityBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> If(StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> IfAsync(StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> IfElse(StateMachineExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                elseActivityCallback)
        {
            ExceptionActivityBinder<TInstance, TData, TException> thenBinder = GetBinder(thenActivityCallback);
            ExceptionActivityBinder<TInstance, TData, TException> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TData, TException>(Event, condition, thenBinder,
                elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, Event, _activities, conditionBinder);
        }

        public ExceptionActivityBinder<TInstance, TData, TException> IfElseAsync(StateMachineAsyncExceptionCondition<TInstance, TData, TException> condition,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                thenActivityCallback,
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>>
                elseActivityCallback)
        {
            ExceptionActivityBinder<TInstance, TData, TException> thenBinder = GetBinder(thenActivityCallback);
            ExceptionActivityBinder<TInstance, TData, TException> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalExceptionActivityBinder<TInstance, TData, TException>(Event, condition, thenBinder,
                elseBinder);

            return new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, Event, _activities, conditionBinder);
        }

        ExceptionActivityBinder<TInstance, TData, TException> GetBinder(
            Func<ExceptionActivityBinder<TInstance, TData, TException>, ExceptionActivityBinder<TInstance, TData, TException>> callback)
        {
            ExceptionActivityBinder<TInstance, TData, TException> binder =
                new CatchExceptionActivityBinder<TInstance, TData, TException>(_machine, Event);
            return callback(binder);
        }
    }
}

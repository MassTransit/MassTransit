namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class DataEventActivityBinder<TInstance, TData> :
        EventActivityBinder<TInstance, TData>
        where TInstance : class, ISaga
        where TData : class
    {
        readonly IActivityBinder<TInstance>[] _activities;
        readonly Event<TData> _event;
        readonly StateMachineCondition<TInstance, TData> _filter;
        readonly StateMachine<TInstance> _machine;

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event, params IActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _activities = activities ?? Array.Empty<IActivityBinder<TInstance>>();
            _machine = machine;
        }

        public DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event, StateMachineCondition<TInstance, TData> filter,
            params IActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _activities = activities ?? Array.Empty<IActivityBinder<TInstance>>();
            _machine = machine;
            _filter = filter;
        }

        DataEventActivityBinder(StateMachine<TInstance> machine, Event<TData> @event, StateMachineCondition<TInstance, TData> filter,
            IActivityBinder<TInstance>[] activities, params IActivityBinder<TInstance>[] appendActivity)
        {
            _activities = new IActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);

            _event = @event;
            _machine = machine;
            _filter = filter;
        }

        Event<TData> EventActivityBinder<TInstance, TData>.Event => _event;

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(IStateMachineActivity<TInstance> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities,
                CreateStateActivityBinder(new SlimActivity<TInstance, TData>(activity)));
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Add(IStateMachineActivity<TInstance, TData> activity)
        {
            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, CreateStateActivityBinder(activity));
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.Catch<T>(
            Func<ExceptionActivityBinder<TInstance, TData, T>, ExceptionActivityBinder<TInstance, TData, T>> activityCallback)
        {
            ExceptionActivityBinder<TInstance, TData, T> binder = new CatchExceptionActivityBinder<TInstance, TData, T>(_machine, _event);

            binder = activityCallback(binder);

            IActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, activityBinder);
        }

        public EventActivityBinder<TInstance, TData> Retry(Action<IRetryConfigurator> configure,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            var configurator = new BehaviorContextRetryConfigurator();
            configure(configurator);

            if (configurator.PolicyFactory == null)
                throw new ConfigurationException("A retry policy must be specified");

            EventActivityBinder<TInstance, TData> activityBinder = GetBinder(activityCallback);

            var retryPolicy = configurator.GetRetryPolicy();

            var binder = new RetryActivityBinder<TInstance, TData>(_event, retryPolicy, activityBinder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, binder);
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.If(StateMachineCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        EventActivityBinder<TInstance, TData> EventActivityBinder<TInstance, TData>.IfAsync(StateMachineAsyncCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public EventActivityBinder<TInstance, TData> IfElse(StateMachineCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> thenActivityCallback,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> elseActivityCallback)
        {
            EventActivityBinder<TInstance, TData> thenBinder = GetBinder(thenActivityCallback);
            EventActivityBinder<TInstance, TData> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance, TData>(_event, condition, thenBinder, elseBinder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, conditionBinder);
        }

        public EventActivityBinder<TInstance, TData> IfElseAsync(StateMachineAsyncCondition<TInstance, TData> condition,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> thenActivityCallback,
            Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> elseActivityCallback)
        {
            EventActivityBinder<TInstance, TData> thenBinder = GetBinder(thenActivityCallback);
            EventActivityBinder<TInstance, TData> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance, TData>(_event, condition, thenBinder, elseBinder);

            return new DataEventActivityBinder<TInstance, TData>(_machine, _event, _filter, _activities, conditionBinder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance, TData>.StateMachine => _machine;

        public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
        {
            if (_filter != null)
                return Enumerable.Repeat(CreateConditionalActivityBinder(), 1);

            return _activities;
        }

        EventActivityBinder<TInstance, TData> GetBinder(Func<EventActivityBinder<TInstance, TData>, EventActivityBinder<TInstance, TData>> activityCallback)
        {
            EventActivityBinder<TInstance, TData> binder = new DataEventActivityBinder<TInstance, TData>(_machine, _event);

            return activityCallback(binder);
        }

        IActivityBinder<TInstance> CreateStateActivityBinder(IStateMachineActivity<TInstance, TData> activity)
        {
            var converterActivity = new DataConverterActivity<TInstance, TData>(activity);

            return new ExecuteActivityBinder<TInstance>(_event, converterActivity);
        }

        IActivityBinder<TInstance> CreateConditionalActivityBinder()
        {
            EventActivityBinder<TInstance, TData> thenBinder = new DataEventActivityBinder<TInstance, TData>(_machine, _event, _activities);
            EventActivityBinder<TInstance, TData> elseBinder = new DataEventActivityBinder<TInstance, TData>(_machine, _event);

            var conditionBinder = new ConditionalActivityBinder<TInstance, TData>(_event, context => _filter(context), thenBinder,
                elseBinder);

            return conditionBinder;
        }
    }
}

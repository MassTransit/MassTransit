namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class TriggerEventActivityBinder<TInstance> :
        EventActivityBinder<TInstance>
        where TInstance : class, ISaga
    {
        readonly IActivityBinder<TInstance>[] _activities;
        readonly Event _event;
        readonly StateMachineCondition<TInstance> _filter;
        readonly StateMachine<TInstance> _machine;

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, params IActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _machine = machine;
            _activities = activities ?? Array.Empty<IActivityBinder<TInstance>>();
        }

        public TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateMachineCondition<TInstance> filter,
            params IActivityBinder<TInstance>[] activities)
        {
            _event = @event;
            _filter = filter;
            _machine = machine;
            _activities = activities ?? Array.Empty<IActivityBinder<TInstance>>();
        }

        TriggerEventActivityBinder(StateMachine<TInstance> machine, Event @event, StateMachineCondition<TInstance> filter,
            IActivityBinder<TInstance>[] activities,
            params IActivityBinder<TInstance>[] appendActivity)
        {
            _event = @event;
            _filter = filter;
            _machine = machine;

            _activities = new IActivityBinder<TInstance>[activities.Length + appendActivity.Length];
            Array.Copy(activities, 0, _activities, 0, activities.Length);
            Array.Copy(appendActivity, 0, _activities, activities.Length, appendActivity.Length);
        }

        Event EventActivityBinder<TInstance>.Event => _event;

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Add(IStateMachineActivity<TInstance> activity)
        {
            IActivityBinder<TInstance> activityBinder = new ExecuteActivityBinder<TInstance>(_event, activity);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.Catch<T>(
            Func<ExceptionActivityBinder<TInstance, T>, ExceptionActivityBinder<TInstance, T>> activityCallback)
        {
            ExceptionActivityBinder<TInstance, T> binder = new CatchExceptionActivityBinder<TInstance, T>(_machine, _event);

            binder = activityCallback(binder);

            IActivityBinder<TInstance> activityBinder = new CatchActivityBinder<TInstance, T>(_event, binder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, activityBinder);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.If(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return IfElse(condition, activityCallback, _ => _);
        }

        EventActivityBinder<TInstance> EventActivityBinder<TInstance>.IfAsync(StateMachineAsyncCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            return IfElseAsync(condition, activityCallback, _ => _);
        }

        public EventActivityBinder<TInstance> IfElse(StateMachineCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback)
        {
            EventActivityBinder<TInstance> thenBinder = GetBinder(thenActivityCallback);
            EventActivityBinder<TInstance> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, condition, thenBinder, elseBinder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, conditionBinder);
        }

        public EventActivityBinder<TInstance> IfElseAsync(StateMachineAsyncCondition<TInstance> condition,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> thenActivityCallback,
            Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> elseActivityCallback)
        {
            EventActivityBinder<TInstance> thenBinder = GetBinder(thenActivityCallback);
            EventActivityBinder<TInstance> elseBinder = GetBinder(elseActivityCallback);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, condition, thenBinder, elseBinder);

            return new TriggerEventActivityBinder<TInstance>(_machine, _event, _filter, _activities, conditionBinder);
        }

        StateMachine<TInstance> EventActivityBinder<TInstance>.StateMachine => _machine;

        public IEnumerable<IActivityBinder<TInstance>> GetStateActivityBinders()
        {
            if (_filter != null)
                return Enumerable.Repeat(CreateConditionalActivityBinder(), 1);

            return _activities;
        }

        EventActivityBinder<TInstance> GetBinder(Func<EventActivityBinder<TInstance>, EventActivityBinder<TInstance>> activityCallback)
        {
            EventActivityBinder<TInstance> binder = new TriggerEventActivityBinder<TInstance>(_machine, _event);
            return activityCallback(binder);
        }

        IActivityBinder<TInstance> CreateConditionalActivityBinder()
        {
            EventActivityBinder<TInstance> thenBinder = new TriggerEventActivityBinder<TInstance>(_machine, _event, _activities);
            EventActivityBinder<TInstance> elseBinder = new TriggerEventActivityBinder<TInstance>(_machine, _event);

            var conditionBinder = new ConditionalActivityBinder<TInstance>(_event, context => _filter(context), thenBinder, elseBinder);

            return conditionBinder;
        }
    }
}

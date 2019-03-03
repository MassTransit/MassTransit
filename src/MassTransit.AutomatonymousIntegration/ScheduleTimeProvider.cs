namespace Automatonymous
{
    using System;


    public delegate DateTime ScheduleTimeProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    public delegate DateTime ScheduleTimeProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;
}

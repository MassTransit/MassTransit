namespace Automatonymous
{
    using System;


    public delegate TimeSpan ScheduleDelayProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    public delegate TimeSpan ScheduleDelayProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;
}

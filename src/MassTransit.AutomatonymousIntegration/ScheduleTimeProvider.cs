namespace Automatonymous
{
    using System;


    public delegate DateTime ScheduleTimeProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    public delegate DateTime ScheduleTimeProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;


    public delegate DateTime ScheduleTimeProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class;
}
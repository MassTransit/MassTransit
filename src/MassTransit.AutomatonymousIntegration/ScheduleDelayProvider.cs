namespace Automatonymous
{
    using System;


    public delegate TimeSpan ScheduleDelayProvider<in TInstance>(ConsumeEventContext<TInstance> context);


    public delegate TimeSpan ScheduleDelayProvider<in TInstance, in TData>(ConsumeEventContext<TInstance, TData> context)
        where TData : class;


    public delegate TimeSpan ScheduleDelayProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class;
}

namespace Automatonymous
{
    using System;


    public delegate DateTime ScheduleTimeExceptionProvider<in TInstance, in TException>(ConsumeExceptionEventContext<TInstance, TException> context);


    public delegate DateTime ScheduleTimeExceptionProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class;
}

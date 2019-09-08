namespace Automatonymous
{
    using System;


    public delegate TimeSpan ScheduleDelayExceptionProvider<in TInstance, in TException>(ConsumeExceptionEventContext<TInstance, TException> context);


    public delegate TimeSpan ScheduleDelayExceptionProvider<in TInstance, in TData, in TException>(ConsumeExceptionEventContext<TInstance, TData, TException> context)
        where TData : class;
}

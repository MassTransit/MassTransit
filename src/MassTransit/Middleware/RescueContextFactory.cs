namespace MassTransit.Middleware
{
    using System;


    public delegate TRescueContext RescueContextFactory<in TContext, out TRescueContext>(TContext context, Exception exception)
        where TContext : class, PipeContext
        where TRescueContext : class, PipeContext;
}

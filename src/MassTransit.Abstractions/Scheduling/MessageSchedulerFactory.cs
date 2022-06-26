namespace MassTransit
{
    public delegate IMessageScheduler MessageSchedulerFactory(ConsumeContext context);
}

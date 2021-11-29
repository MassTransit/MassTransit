namespace MassTransit.HangfireIntegration
{
    public interface IHashedScheduleId
    {
        string? HashId { get; }
    }
}

namespace MassTransit;

public interface OneTimeContext<TPayload>
    where TPayload : class
{
    void Evict();
}

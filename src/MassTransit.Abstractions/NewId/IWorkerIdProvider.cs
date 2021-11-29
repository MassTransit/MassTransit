namespace MassTransit
{
    public interface IWorkerIdProvider
    {
        byte[] GetWorkerId(int index);
    }
}

namespace MassTransit
{
    public interface IProcessIdProvider
    {
        byte[] GetProcessId();
    }
}

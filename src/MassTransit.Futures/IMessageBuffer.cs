namespace MassTransit
{
    using System.Threading.Tasks;


    public interface IMessageBuffer
    {
        Task Add<T>(IMessageEvent<T> messageEvent)
            where T : class;
    }
}
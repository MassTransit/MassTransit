namespace MassTransit.Initializers
{
    using System.Threading.Tasks;


    public interface IInitializerVariable<T>
    {
        Task<T> GetValue<TMessage>(InitializeContext<TMessage> context)
            where TMessage : class;
    }
}

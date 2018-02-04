namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    /// <summary>
    /// Converts a <see cref="Task{T}"/> to {T} by awaiting the result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class TaskPropertyConverter<TResult> :
        IPropertyConverter<TResult, Task<TResult>>
    {
        Task<TResult> IPropertyConverter<TResult, Task<TResult>>.Convert<TMessage>(InitializeContext<TMessage> context, Task<TResult> input)
        {
            return input;
        }
    }
}

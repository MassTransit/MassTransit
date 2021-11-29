namespace MassTransit
{
    using System.Threading.Tasks;


    public delegate Task InlineFilterMethod<T>(T context, IPipe<T> next)
        where T : class, PipeContext;
}

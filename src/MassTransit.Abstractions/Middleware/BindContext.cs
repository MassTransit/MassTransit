namespace MassTransit
{
    /// <summary>
    /// The binding of a value to the context, which is a fancy form of Tuple
    /// </summary>
    /// <typeparam name="TLeft">The pipe context type</typeparam>
    /// <typeparam name="TRight">The source context type</typeparam>
    public interface BindContext<out TLeft, out TRight> :
        PipeContext
        where TLeft : class, PipeContext
        where TRight : class
    {
        TLeft Left { get; }
        TRight Right { get; }
    }
}

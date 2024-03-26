namespace MassTransit.Metadata
{
    public interface IActivationType<out TResult>
    {
        TResult ActivateType<T>()
            where T : class;
    }


    public interface IActivationType<out TResult, in T1>
    {
        TResult ActivateType<T>(T1 arg1)
            where T : class;
    }


    public interface IActivationType<out TResult, in T1, in T2>
    {
        TResult ActivateType<T>(T1 arg1, T2 arg2)
            where T : class;
    }
}

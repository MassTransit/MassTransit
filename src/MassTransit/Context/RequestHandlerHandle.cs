namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface RequestHandlerHandle :
        ConnectHandle
    {
        void TrySetException(Exception exception);
        void TrySetCanceled();

        Task<T> GetTask<T>();
    }
}

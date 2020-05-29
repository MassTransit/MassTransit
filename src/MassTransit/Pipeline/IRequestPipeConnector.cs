namespace MassTransit.Pipeline
{
    using System;
    using GreenPipes;


    /// <summary>
    /// Connect a request pipe to the pipeline
    /// </summary>
    public interface IRequestPipeConnector
    {
        /// <summary>
        /// Connect the consume pipe to the pipeline for messages with the specified RequestId header
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestId"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class;
    }


    /// <summary>
    /// A connector for a pipe by request id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRequestPipeConnector<out T>
        where T : class
    {
        /// <summary>
        /// Connect the consume pipe to the pipeline for messages with the specified RequestId header
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        ConnectHandle ConnectRequestPipe(Guid requestId, IPipe<ConsumeContext<T>> pipe);
    }
}

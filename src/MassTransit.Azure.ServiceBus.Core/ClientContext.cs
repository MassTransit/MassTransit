namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;


    /// <summary>
    /// The client context is used to access the queue/subscription/topic client.
    /// </summary>
    public interface ClientContext :
        NamespaceContext
    {
        /// <summary>
        /// The input address for the client/transport
        /// </summary>
        Uri InputAddress { get; }

        /// <summary>
        /// The path of the messaging entity
        /// </summary>
        string EntityPath { get; }

        /// <summary>
        /// Register an message handler for the client
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="exceptionHandler"></param>
        void OnMessageAsync(Func<IReceiverClient, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler);

        /// <summary>
        /// Register a message session handler
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="exceptionHandler"></param>
        void OnSessionAsync(Func<IMessageSession, Message, CancellationToken, Task> callback, Func<ExceptionReceivedEventArgs, Task> exceptionHandler);

        /// <summary>
        /// Close down the message handler on the received
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CloseAsync(CancellationToken cancellationToken);
    }
}

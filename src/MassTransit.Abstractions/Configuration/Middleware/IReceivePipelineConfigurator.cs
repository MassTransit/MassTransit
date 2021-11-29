namespace MassTransit
{
    using System;


    public interface IReceivePipelineConfigurator
    {
        /// <summary>
        /// Configure the Receive pipeline
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureReceive(Action<IReceivePipeConfigurator> callback);

        /// <summary>
        /// Configure the dead letter pipeline, which is called if the message is not consumed
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureDeadLetter(Action<IPipeConfigurator<ReceiveContext>> callback);

        /// <summary>
        /// Configure the exception pipeline, which is called if there are uncaught consumer exceptions
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureError(Action<IPipeConfigurator<ExceptionReceiveContext>> callback);

        /// <summary>
        /// Configure the transport options
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureTransport(Action<ITransportConfigurator> callback);
    }
}

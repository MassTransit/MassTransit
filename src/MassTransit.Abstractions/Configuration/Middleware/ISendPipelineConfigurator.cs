namespace MassTransit
{
    using System;


    public interface ISendPipelineConfigurator
    {
        /// <summary>
        /// Configure the Send pipeline
        /// </summary>
        /// <param name="callback"></param>
        void ConfigureSend(Action<ISendPipeConfigurator> callback);
    }
}

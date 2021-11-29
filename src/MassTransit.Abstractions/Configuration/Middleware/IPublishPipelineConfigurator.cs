namespace MassTransit
{
    using System;


    public interface IPublishPipelineConfigurator
    {
        /// <summary>
        /// Configure the Publish pipeline
        /// </summary>
        /// <param name="callback"></param>
        void ConfigurePublish(Action<IPublishPipeConfigurator> callback);
    }
}

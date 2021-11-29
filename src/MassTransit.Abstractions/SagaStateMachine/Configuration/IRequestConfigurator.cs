namespace MassTransit
{
    using System;


    public interface IRequestConfigurator
    {
        /// <summary>
        /// Sets the service address of the request handler
        /// </summary>
        Uri ServiceAddress { set; }

        /// <summary>
        /// Sets the request timeout
        /// </summary>
        TimeSpan Timeout { set; }
    }
}

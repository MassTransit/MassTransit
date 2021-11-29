namespace MassTransit
{
    using System;


    public interface IEndpointConventionCache<in TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Returns the endpoint address for the message
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        bool TryGetEndpointAddress(out Uri address);
    }
}

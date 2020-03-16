namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;


    public interface PublishSettings :
        EntitySettings
    {
        /// <summary>
        /// Returns the send address for the settings
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        Uri GetSendAddress(Uri hostAddress);
    }
}

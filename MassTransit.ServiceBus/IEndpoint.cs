using System;

namespace MassTransit.ServiceBus
{
	/// <summary>
	/// IEndpoint is implemented by an endpoint. An endpoint is an addressable location on the network.
	/// </summary>
    public interface IEndpoint :
        IDisposable
    {
		/// <summary>
		/// The address of the endpoint, in URI format
		/// </summary>
		Uri Uri { get; }
    }
}
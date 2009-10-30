namespace GatewayService.Interfaces
{
	public interface RetrieveOrderDetails
	{
		string OrderId { get; }
		string CustomerId { get; }
	}
}
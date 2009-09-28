namespace BusinessWebService
{
	using System;
	using System.ComponentModel;
	using System.Web.Services;
	using Inventory.Messages;
	using Magnum;
	using Magnum.DateTimeExtensions;
	using MassTransit;
	using MassTransit.Util;

	/// <summary>
	/// Summary description for InventoryService
	/// </summary>
	[WebService(Namespace = "http://masstransit.googlecode.com/Samples/BusinessWebService")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class InventoryService :
		WebService
	{
		private static readonly Magnum.Mapper<PartInventoryLevelStatus, InventoryLevel> _mapper;
		private IServiceBus _bus;

		private string _partNumber;
		private Guid _requestId;
		private PartInventoryLevelStatus _status;

		static InventoryService()
		{
			_mapper = new Magnum.Mapper<PartInventoryLevelStatus, InventoryLevel>();
			_mapper.From(x => x.PartNumber).To(y => y.PartNumber);
			_mapper.From(x => x.OnHand).To(y => y.QuantityOnHand);
			_mapper.From(x => x.OnOrder).To(y => y.QuantityOnOrder);
		}

		public InventoryService()
		{
			_bus = IoC.Container.Resolve<IServiceBus>("client");
		}

		~InventoryService()
		{
			IoC.Container.Release(_bus);
		}

		[WebMethod]
		public IAsyncResult BeginCheckInventory(string partNumber, AsyncCallback callback, object state)
		{
			_partNumber = partNumber;
			_requestId = CombGuid.Generate();

			return _bus.MakeRequest(bus => bus.Publish(new QueryInventoryLevel(_requestId, partNumber), context => context.SendResponseTo(bus)))
				.When<PartInventoryLevelStatus>().RelatedTo(_partNumber).IsReceived(message =>
					{
						_status = message;
					})
				.TimeoutAfter(30.Seconds())
				.BeginSend(callback, state);
		}

		[WebMethod]
		public InventoryLevel EndCheckInventory(IAsyncResult asyncResult)
		{
			if (_status == null)
				throw new ApplicationException("The service did not respond quickly enough.");

			return _mapper.Transform(_status);
		}
	}

	public class InventoryLevel
	{
		public string PartNumber { get; set; }

		public int QuantityOnHand { get; set; }

		public int QuantityOnOrder { get; set; }
	}
}
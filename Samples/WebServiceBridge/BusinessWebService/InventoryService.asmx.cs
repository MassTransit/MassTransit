namespace BusinessWebService
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Web.Services;
    using Inventory.Messages;
    using Magnum.Common;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Services.Timeout.Messages;
    using MassTransit.ServiceBus.Util;

    /// <summary>
    /// Summary description for InventoryService
    /// </summary>
    [WebService(Namespace = "http://masstransit.googlecode.com/Samples/BusinessWebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class InventoryService : 
        WebService,
        Consumes<PartInventoryLevelStatus>.For<string>,
        Consumes<TimeoutExpired>.For<Guid>
    {
        private static readonly Mapper<PartInventoryLevelStatus, InventoryLevel> _mapper;

        private string _partNumber;
        private PartInventoryLevelStatus _status;
        private ServiceBusRequest<InventoryService> _request;
        private IServiceBus _bus;
        private Guid _requestId;

        static InventoryService()
        {
            _mapper = new Mapper<PartInventoryLevelStatus, InventoryLevel>();
            _mapper.From(x => x.PartNumber).To(y => y.PartNumber);
            _mapper.From(x => x.OnHand).To(y => y.QuantityOnHand);
            _mapper.From(x => x.OnOrder).To(y => y.QuantityOnOrder);
        }

        public InventoryService()
        {
            _bus = IoC.Container.Resolve<IServiceBus>("client");
        }

        [WebMethod]
        public IAsyncResult BeginCheckInventory(string partNumber, AsyncCallback callback, object state)
        {
            _partNumber = partNumber;
            _requestId = CombGuid.NewCombGuid();

            _request = _bus.Request().From(this).WithCallback(callback, state);

            QueryInventoryLevel queryInventoryLevel = new QueryInventoryLevel(_requestId, partNumber);
            _request.Send(queryInventoryLevel);

            ScheduleTimeout scheduleTimeout = new ScheduleTimeout(_requestId, 30.Seconds().FromNow());
            _bus.Publish(scheduleTimeout);

            return _request;
        }

        [WebMethod]
        public InventoryLevel EndCheckInventory(IAsyncResult asyncResult)
        {
            if (_status == null)
                throw new ApplicationException("The service did not respond quickly enough.");

            return _mapper.Transform(_status);
        }

        public void Consume(PartInventoryLevelStatus message)
        {
            _status = message;

            _bus.Publish(new CancelTimeout(_requestId));

            _request.Complete();
        }

        string CorrelatedBy<string>.CorrelationId
        {
            get { return _partNumber; }
        }

        public void Consume(TimeoutExpired message)
        {
            _request.Complete();
        }

        Guid CorrelatedBy<Guid>.CorrelationId
        {
            get { return _requestId; }
        }
    }

    public class InventoryLevel
    {
        public string PartNumber { get; set; }

        public int QuantityOnHand { get; set; }

        public int QuantityOnOrder { get; set; }
    }
}
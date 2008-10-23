namespace BusinessWebService
{
    using System.ComponentModel;
    using System.Web.Services;
    using Inventory.Messages;
    using Magnum.Common;

    /// <summary>
    /// Summary description for InventoryService
    /// </summary>
    [WebService(Namespace = "http://masstransit.googlecode.com/Samples/BusinessWebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class InventoryService : WebService
    {
        private static readonly Mapper<PartInventoryLevelStatus, InventoryLevel> _mapper;

        static InventoryService()
        {
            _mapper = new Mapper<PartInventoryLevelStatus, InventoryLevel>();
            _mapper.From(x => x.PartNumber).To(y => y.PartNumber);
            _mapper.From(x => x.OnHand).To(y => y.QuantityOnHand);
            _mapper.From(x => x.OnOrder).To(y => y.QuantityOnOrder);
        }

        [WebMethod]
        public InventoryLevel CheckInventory(string partNumber)
        {
            PartInventoryLevelStatus status = new PartInventoryLevelStatus(partNumber, -1, -1);


            return _mapper.Transform(status);
        }
    }

    public class InventoryLevel
    {
        public string PartNumber { get; set; }

        public int QuantityOnHand { get; set; }

        public int QuantityOnOrder { get; set; }
    }
}
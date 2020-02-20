using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransit.Analyzers
{

    public interface OrderItem
    {
        Guid OrderId { get; }
        string ItemNumber { get; }
    }

    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime OrderDate { get; }
        string OrderNumber { get; }
        decimal OrderAmount { get; }
        OrderItem[] OrderItems { get; }
    }

    public interface OrderSubmitted
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
    }

    public interface CheckOrderStatus
    {
        string OrderId { get; }
    }

    public interface OrderStatusResult
    {
        string OrderId { get; }
        DateTime Timestamp { get; }
        short StatusCode { get; }
        string StatusText { get; }
    }

    public interface UpdateCustomerAddress
    {
        Guid CommandId { get; }
        DateTime Timestamp { get; }
        string CustomerId { get; }
        string HouseNumber { get; }
        string Street { get; }
        string City { get; }
        string State { get; }
        string PostalCode { get; }
    }

    public interface OrderUpdated
    {
        Guid CorrelationId { get; }
        DateTime Timestamp { get; }
        Guid OrderId { get; }
        //Customer Customer { get; }
    }






    public interface ICommand
    {
        Guid CommandId { get; }
        Guid StreamId { get; }
    }

    public interface IAddress
    {
        string Street { get; }
        string Place { get; }
    }

    public interface IIdentification
    {
        string Type { get; }
        string IssuingCountry { get; }
        string Number { get; }
    }

    public interface ICreateCommand : ICommand
    {
        string Name { get; }
        IAddress BillingAddress { get; }
        IAddress DeliveryAddress { get; }
        IReadOnlyList<IIdentification> Identifications { get; }
        IReadOnlyList<IIdentification> Documents { get; }
    }

    public class AddressModel
    {
        public string Street { get; }
        public string Place { get; }
    }

    public class IdentificationModel
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
    }

    public class CreateRequest
    {
        public string Name { get; }
        public AddressModel BillingAddress { get; } = new AddressModel();
        public AddressModel DeliveryAddress { get; } = new AddressModel();
        public IReadOnlyList<IdentificationModel> Identifications { get; } = new List<IdentificationModel>();
        public IReadOnlyList<IdentificationModel> Documents { get; } = new List<IdentificationModel>();
    }

    class Program
    {
        static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingInMemory(cfg => { });
            
            //var requestClient = busControl.CreateRequestClient<OrderRequest>(null);

            //var request = requestClient.Create(new
            //{
            //});


            var sendEndpoint = await busControl.GetSendEndpoint(new Uri("queue:queue_name"));
            
            await sendEndpoint.Send<ICreateCommand>(new
            {
                Name = string.Empty,
                BillingAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = new[] 
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                },
                Documents = new[] 
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                },
                CommandId = Guid.NewGuid(),
                StreamId = Guid.NewGuid()
            });

            var request = new CreateRequest();

            await sendEndpoint.Send<ICreateCommand>(new
            {
                request.Name,
                BillingAddress = new
                {
                    request.BillingAddress.Place,
                    request.BillingAddress.Street
                },
                DeliveryAddress = new
                {
                    request.DeliveryAddress.Place,
                    request.DeliveryAddress.Street
                },
                Identifications = request.Identifications.Select(i => new
                {
                    i.Type,
                    i.IssuingCountry,
                    i.Number
                }).ToList(),
                Documents = request.Documents.Select(d => new
                {
                    d.Type,
                    d.IssuingCountry,
                    d.Number
                }).ToList(),
                CommandId = Guid.NewGuid(),
                StreamId = Guid.NewGuid(),
            });
        }
    }
}

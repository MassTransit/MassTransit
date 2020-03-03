using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Analyzers
{
    using MassTransit;


    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<OrderItem> OrderItems { get; }
    }

    public interface SubmitOrder
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<OrderItem> OrderItems { get; }
    }

    public interface OrderItem
    {
        Guid Id { get; }
        Product Product { get; }
        int Quantity { get; }
        decimal Price { get; }
    }

    public interface Product
    {
        string Name { get; }
        string Category { get; }
    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public ProductDto Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }

    public interface CheckOrderStatus
    {
        Guid OrderId { get; }
    }

    public interface OrderStatusResult
    {
        Guid OrderId { get; }
        string Status { get; }
    }
         
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            //var requestClient = bus.CreateRequestClient<CheckOrderStatus>(null);

            //using (var request = requestClient.Create(new
            //{
            //    OrderId = default(string)
            //}))
            //{
            //    var response = await request.GetResponse<OrderStatusResult>();
            //    var result = response.Message;
            //}

            //var response = await bus.Request<CheckOrderStatus, OrderStatusResult>(new
            //{
            //});

            //var response = await bus.Request<CheckOrderStatus, OrderStatusResult>(null, new
            //{
            //});
            //var result = response.Message;

            //var publishRequestClient = bus.CreatePublishRequestClient<CheckOrderStatus, OrderStatusResult>(TimeSpan.FromSeconds(1));
            //var result = await publishRequestClient.Request(new { });

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = "Customer",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = "Product",
                            Category = "Category"
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            });

            var sendEndpoint = await bus.GetSendEndpoint(null);

            await sendEndpoint.Send<SubmitOrder>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = "Customer",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = "Product",
                            Category = "Category"
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            });

            var order = new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerId = "Customer",
                OrderItems =
                {
                    new OrderItemDto
                    {
                        Id = Guid.NewGuid(),
                        Product = new ProductDto
                        {
                            Name ="Product",
                            Category = "Category"
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };

            await bus.Publish<OrderSubmitted>(new
            {
                order.Id,
                order.CustomerId,
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.Id,
                    Product = new
                    {
                        item.Product.Name,
                        item.Product.Category
                    },
                    item.Quantity,
                    item.Price
                }).ToList()
            });
        }
    }
}

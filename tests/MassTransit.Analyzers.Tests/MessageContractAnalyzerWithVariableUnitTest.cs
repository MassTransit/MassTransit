namespace MassTransit.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;


    public class MessageContractAnalyzerWithVariableUnitTest : CodeFixVerifier
    {
        readonly string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
";

        readonly string MessageContracts = @"
namespace ConsoleApplication1
{
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
        Uri Category { get; }
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
}
";

        readonly string ExtraMessageContracts = @"
namespace ConsoleApplication1
{
    public interface SubmitOrderEnumerable
    {
        Guid Id { get; }
        string CustomerId { get; }
        IEnumerable<OrderItem> OrderItems { get; }
    }
}
";

        readonly string RecordContracts = @"
namespace ConsoleApplication1
{
    public record OrderSubmissionReceived
    {
        public Guid Id { get; init; }
        public string CustomerId { get; init; }
    }
}
";

        readonly string RecursiveContracts = @"namespace ConsoleApplication1
{
    public interface Foo
    {
        IList<Foo> Children { get; }
        Bar Bar { get; }
    }

    public interface Bar
    {

    }
}
";

        readonly string GenericMessageContracts = @"
namespace ConsoleApplication1
{
    public interface Command
    {
    }

    public interface Up<T>
        where T : class
    {
        Guid InstanceId { get; }
        Uri InstanceAddress { get; }
    }

    public interface Link<T>
        where T : class
    {
        Guid ClientId { get; }
    }
}
";

        readonly string MessageContractsDifferentNamespace = @"
namespace ConsoleApplication1.Messages
{
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
        Uri Category { get; }
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
}
";

        readonly string Dtos = @"
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
";

        readonly string DtosIncompatibe = @"
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
        public int Category { get; set; }
    }
";


        [Test]
        public void WhenActivatingGenericContractAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public Guid StreamId { get; }
    }

    public interface IProjectionUpdatedNotification : INotification
    {
    }

    class Program
    {
        static async Task Main()
        {
            await PublishNotification<IProjectionUpdatedNotification>(Guid.Empty);
        }

        private static Task PublishNotification<T>(Guid streamId) where T : class, INotification
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var msg = new
            {
            };
            return bus.Publish<T>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'INotification'. The following properties are missing: StreamId.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 29, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public Guid StreamId { get; }
    }

    public interface IProjectionUpdatedNotification : INotification
    {
    }

    class Program
    {
        static async Task Main()
        {
            await PublishNotification<IProjectionUpdatedNotification>(Guid.Empty);
        }

        private static Task PublishNotification<T>(Guid streamId) where T : class, INotification
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var msg = new
            {

                StreamId = default(Guid)
            };
            return bus.Publish<T>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenActivatingGenericContractAreStructurallyCompatibleAndNoMissingProperties_WithVariables_ShouldHaveNoDiagnostics()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public Guid StreamId { get; }
    }

    public interface IProjectionUpdatedNotification : INotification
    {
    }

    class Program
    {
        static async Task Main()
        {
            await PublishNotification<IProjectionUpdatedNotification>(Guid.Empty);
        }

        private static Task PublishNotification<T>(Guid streamId) where T : class, INotification
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var msg = new
            {
                StreamId = streamId
            };
            return bus.Publish<T>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenActivatingGenericContractAreStructurallyIncompatibleAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public int StreamId { get; }
    }

    public interface IProjectionUpdatedNotification : INotification
    {
    }

    class Program
    {
        static async Task Main()
        {
            await PublishNotification<IProjectionUpdatedNotification>(Guid.Empty);
        }

        private static Task PublishNotification<T>(Guid streamId) where T : class, INotification
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                StreamId = streamId
            };
            return bus.Publish<T>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'INotification'. The following properties of the anonymous type are incompatible: StreamId.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 30, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + Dtos + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var order = new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerId = ""Customer"",
                OrderItems =
                {
                    new OrderItemDto
                    {
                        Id = Guid.NewGuid(),
                        Product = new ProductDto
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };

            var msg = new
            {
                order.Id,
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.Id,
                    Product = new
                    {
                        item.Product.Name
                    },
                    item.Quantity
                }).ToList()
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId, OrderItems.Product.Category, OrderItems.Price.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 99, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + Dtos + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var order = new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerId = ""Customer"",
                OrderItems =
                {
                    new OrderItemDto
                    {
                        Id = Guid.NewGuid(),
                        Product = new ProductDto
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };

            var msg = new
            {
                order.Id,
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.Id,
                    Product = new
                    {
                        item.Product.Name
,
                        Category = default(Uri)
                    },
                    item.Quantity
,
                    Price = default(decimal)
                }).ToList()
,
                CustomerId = default(string)
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + Dtos + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var order = new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerId = ""Customer"",
                OrderItems =
                {
                    new OrderItemDto
                    {
                        Id = Guid.NewGuid(),
                        Product = new ProductDto
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };

            var msg = new
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
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyIncompatibleAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + DtosIncompatibe + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var order = new OrderDto
            {
                Id = Guid.NewGuid(),
                CustomerId = ""Customer"",
                OrderItems =
                {
                    new OrderItemDto
                    {
                        Id = Guid.NewGuid(),
                        Product = new ProductDto
                        {
                            Name = ""Product"",
                            Category = 1
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };

            var msg = new
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
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Category.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 99, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenCreateRequestTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var requestClient = bus.CreateRequestClient<CheckOrderStatus>(null);

            var msg = new
            {
            };
            using (var request = requestClient.Create(msg))
            {
                var response = await request.GetResponse<OrderStatusResult>();
                var result = response.Message;
            }
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'CheckOrderStatus'. The following properties are missing: OrderId.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 59, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenGetResponseTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var requestClient = bus.CreateRequestClient<CheckOrderStatus>(null);

            var msg = new {};
            var response = await requestClient.GetResponse<OrderStatusResult>(msg);
            var result = response.Message;            
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'CheckOrderStatus'. The following properties are missing: OrderId.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 59, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenCreateRequestTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveTheNoDiagnostic()
        {
            var test = Usings + GenericMessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main<TMessage>()
            where TMessage : class
        {
            Guid ClientId = NewId.NextGuid();
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var requestClient = bus.CreateRequestClient<Link<TMessage>>();

            var msg = new{ClientId};
            var response = await requestClient.GetResponse<Up<TMessage>>(msg);
            var result = response.Message;
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenCreateRequestTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveTheRightDiagnostic()
        {
            var test = Usings + GenericMessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main<TMessage>()
            where TMessage : class
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var requestClient = bus.CreateRequestClient<Link<TMessage>>();

            var msg = new{};
            var response = await requestClient.GetResponse<Up<TMessage>>(msg);
            var result = response.Message;
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'Link'. The following properties are missing: ClientId.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 38, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenCreateRequestTypesAreStructurallyCompatibleAndNoMissingProperty_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var requestClient = bus.CreateRequestClient<CheckOrderStatus>(null);

            var msg = new
            {
                OrderId = ""3F70AEF5-F840-4BF8-8012-F2CC55697EB0""
            };
            using (var request = requestClient.Create(msg))
            {
                var response = await request.GetResponse<OrderStatusResult>();
                var result = response.Message;
            }
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenMessageContractHasNamespaceAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product""
                        },
                        Quantity = 10
                    }
                }
            };
            await bus.Publish<Messages.OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId, OrderItems.Product.Category, OrderItems.Price.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product""
,
Category = default(Uri) },
                        Quantity = 10
,
Price = default(decimal) }
                }
,
                CustomerId = default(string)
            };
            await bus.Publish<Messages.OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenMessageContractHasNamespaceAreStructurallyCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<Messages.OrderSubmitted>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenMessageContractHasNamespaceAreStructurallyIncompatibleAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = 42,
                CustomerId = ""27"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<Messages.OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: Id.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenMessageContractHasNullableAreStructurallyCompatibleAndMissingNullableProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        int Quantity { get; }
        decimal? Price { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                Quantity = 10
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: Price.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 23, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        int Quantity { get; }
        decimal? Price { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                Quantity = 10
,
                Price = default(decimal?)
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenPublishTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {                
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: Id, CustomerId, OrderItems.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenSendTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var sendEndpoint = await bus.GetSendEndpoint(null);

            var msg = new
            {
            };
            await sendEndpoint.Send<SubmitOrder>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'SubmitOrder'. The following properties are missing: Id, CustomerId, OrderItems.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 59, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenSimpleArrayTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<Guid> OrderItems { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 23, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<Guid> OrderItems { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
,
                OrderItems = new[] { default(Guid) }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenSimpleArrayTypesAreStructurallyCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<Guid> OrderItems { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = Array.Empty<Guid>()
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenSimpleArrayTypesAreStructurallyIncompatibleAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<Guid> OrderItems { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = Array.Empty<int>()
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 23, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypeNotValidStructure_WithVariable_ShouldHaveDiagnostic_ButItLooksGoodToMe()
        {
            var test = Usings + Dtos + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""category:General""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderDto>(msg);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingMultiplePropertiesAtDifferentNodes_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product""
                        },
                        Quantity = 10
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId, OrderItems.Product.Category, OrderItems.Price.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product""
,
Category = default(Uri) },
                        Quantity = 10
,
Price = default(decimal) }
                }
,
                CustomerId = default(string)
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingNestedArrayTypeProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
,
                OrderItems = new[] {
                    new
                    {
                        Id = default(Guid),
                        Product = new
                        {
                            Name = default(string),
                            Category = default(Uri)
                        },
                        Quantity = default(int),
                        Price = default(decimal)
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingNestedTypeProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.Product.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Quantity = 10,
                        Price = 10.0m
,
Product = new { Name = default(string),
Category = default(Uri) } }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingProperty_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
,
                CustomerId = default(string)
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingPropertyInNestedArrayType_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.Price.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10
,
Price = default(decimal) }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingPropertyInNestedType_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.Product.Category.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product""
,
Category = default(Uri) },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleWithAsyncAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = 1,
                OrderItems = new[]
                {
                    new
                    {
                        Id = Task.FromResult<Guid>(NewId.NextGuid()),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatibleWithDunderProperty_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                __TimeToLive = 15000,
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = 27,
                CustomerId = 1,
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: Id.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleAtDifferentNodesAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = 1,
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = 1
                        },
                        Quantity = 10,
                        Price = 10.0
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Category, OrderItems.Price.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleAtNestedArrayTypePropertyAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Price.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleAtNestedTypePropertyAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = 1
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Category.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                },
                Amount = 100.0m
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: Amount.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAtNestedArrayTypePropertyAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m,
                        Amount = 100.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Amount.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAtNestedTypePropertyAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = NewId.NextGuid(),
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category"",
                            Price = 10.0m
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await bus.Publish<OrderSubmitted>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Price.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 58, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesWithVariablesAreStructurallyCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var sendEndpoint = await bus.GetSendEndpoint(null);

            var msg = new
            {
                Id = InVar.Id,
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = InVar.Id,
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await sendEndpoint.Send<SubmitOrder>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenTypesWithVariablesAreEnumerableCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + ExtraMessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var sendEndpoint = await bus.GetSendEndpoint(null);

            var msg = new
            {
                Id = InVar.Id,
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = InVar.Id,
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await sendEndpoint.Send<SubmitOrderEnumerable>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenRecordsWithVariablesAreStructurallyCompatibleAndNoMissingProperties_WithVariable_ShouldHaveNoDiagnostics()
        {
            var test = Usings + RecordContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var sendEndpoint = await bus.GetSendEndpoint(null);

            var msg = new
            {
                Id = InVar.Id,
                CustomerId = ""Customer""
            };
            await sendEndpoint.Send<OrderSubmissionReceived>(msg);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void WhenTypesWithVariablesAreStructurallyIncompatibleAndNoMissingProperties_WithVariable_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });
            var sendEndpoint = await bus.GetSendEndpoint(null);

            var msg = new
            {
                Id = InVar.Timestamp,
                CustomerId = ""Customer"",
                OrderItems = new[]
                {
                    new
                    {
                        Id = InVar.Id,
                        Product = new
                        {
                            Name = ""Product"",
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            };
            await sendEndpoint.Send<SubmitOrder>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'SubmitOrder'. The following properties of the anonymous type are incompatible: Id.",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 59, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenRecursiveTypesAreStructurallyCompatibleAndMissingPropertyInNestedType_WithVariable_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + RecursiveContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {
            };
            await bus.Publish<Foo>(msg);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'Foo'. The following properties are missing: Children, Bar.",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] { new DiagnosticResultLocation("Test0.cs", 29, 23) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + RecursiveContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var msg = new
            {

                Children = new[] {
                    new
                    {
                        Children = Array.Empty<Foo>(),
                        Bar = new { }
                    }
                },
                Bar = new { }
            };
            await bus.Publish<Foo>(msg);
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MessageContractCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MessageContractAnalyzer();
        }
    }
}

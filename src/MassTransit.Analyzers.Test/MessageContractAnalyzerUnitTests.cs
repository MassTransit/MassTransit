using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace MassTransit.Analyzers.MessageContractAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        private readonly string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
";

        private readonly string MessageContracts = @"
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
        string Category { get; }
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

        private readonly string MessageContractsDifferentNamespace = @"
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
        string Category { get; }
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

        private readonly string Dtos = @"
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

        private readonly string DtosIncompatibe = @"
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

        [TestMethod]
        public void WhenPublishTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
            {
                }
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: Id, CustomerId, OrderItems",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

        }

        [TestMethod]
        public void WhenSendTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnostic()
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

            await sendEndpoint.Send<SubmitOrder>(new
            {
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'SubmitOrder'. The following properties are missing: Id, CustomerId, OrderItems",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 59, 50)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenCreateRequestTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnostic()
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

            using (var request = requestClient.Create(new
            {
            }))
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
                Message = "Anonymous type is missing properties that are in the message contract 'CheckOrderStatus'. The following properties are missing: OrderId",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 59, 55)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [TestMethod]
        public void WhenTypeNotValidStructure_ShouldHaveDiagnostic()
        {
            var test = Usings + Dtos + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderDto>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0002",
                Message = "Message contract 'OrderDto' does not have a valid structure",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 37, 41)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

        }


        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }


        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: CustomerId",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: Amount",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleAtNestedTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Category",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAtNestedTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Price",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleAtNestedArrayTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Price",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAtNestedArrayTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Amount",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleAtDifferentNodesAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: CustomerId, OrderItems.Product.Category, OrderItems.Price",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingPropertyInNestedType_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.Product.Category",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
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
Category = default(string) },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingPropertyInNestedArrayType_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.Price",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingNestedTypeProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems.Product",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
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
Category = default(string) } }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingNestedArrayTypeProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
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
                            Category = default(string)
                        },
                        Quantity = default(int),
                        Price = default(decimal)
                    }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }
        
        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingMultiplePropertiesAtDifferentNodes_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId, OrderItems.Product.Category, OrderItems.Price",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
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
Category = default(string) },
                        Quantity = 10
,
Price = default(decimal) }
                }
,
                CustomerId = default(string)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
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
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
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
";

            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems.Product.Category",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 99, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
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

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId, OrderItems.Product.Category, OrderItems.Price",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 99, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
            {
                order.Id,
                OrderItems = order.OrderItems.Select(item => new
                {
                    item.Id,
                    Product = new
                    {
                        item.Product.Name
,
                        Category = default(string)
                    },
                    item.Quantity
,
                    Price = default(decimal)
                }).ToList()
,
                CustomerId = default(string)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void WhenSimpleArrayTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
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

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = Array.Empty<Guid>()
            });
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenSimpleArrayTypesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
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

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer"",
                OrderItems = Array.Empty<int>()
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 23, 47)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenSimpleArrayTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
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

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: OrderItems",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 23, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""Customer""
,
                OrderItems = new[] { default(Guid) }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void WhenMessageContractHasNamespaceAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<Messages.OrderSubmitted>(new
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
            });
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenMessageContractHasNamespaceAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<Messages.OrderSubmitted>(new
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
                            Category = ""Category""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: CustomerId",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 56)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenMessageContractHasNamespaceAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings + MessageContractsDifferentNamespace + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<Messages.OrderSubmitted>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: CustomerId, OrderItems.Product.Category, OrderItems.Price",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 58, 56)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings + MessageContractsDifferentNamespace  + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<Messages.OrderSubmitted>(new
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
Category = default(string) },
                        Quantity = 10
,
Price = default(decimal) }
                }
,
                CustomerId = default(string)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void WhenMessageContractHasNullableAreStructurallyCompatibleAndMissingNullableProperty_ShouldHaveDiagnosticAndCodeFix()
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

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                Quantity = 10
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: Price",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 23, 47)
                        }
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

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                Quantity = 10
,
                Price = default(decimal?)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void WhenActivatingGenericContractAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
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

            return bus.Publish<T>(new
            {
                StreamId = streamId
            });
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenActivatingGenericContractAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
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

            return bus.Publish<T>(new
            {
                StreamId = streamId
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'INotification'. The following properties of the anonymous type are incompatible: StreamId",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 30, 35)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void WhenActivatingGenericContractAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
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

            return bus.Publish<T>(new
            {
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'INotification'. The following properties are missing: StreamId",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 30, 35)
                        }
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

            return bus.Publish<T>(new
            {

                StreamId = default(Guid)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }


        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleWithDunderProperty_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
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
            });
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenTypesWithVariablesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
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

            await sendEndpoint.Send<SubmitOrder>(new
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
            });
        }
    }
}
";
            
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenTypesWithVariablesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
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

            await sendEndpoint.Send<SubmitOrder>(new
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
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'SubmitOrder'. The following properties of the anonymous type are incompatible: Id",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 59, 50)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MessageContractAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MessageContractAnalyzerAnalyzer();
        }
    }
}

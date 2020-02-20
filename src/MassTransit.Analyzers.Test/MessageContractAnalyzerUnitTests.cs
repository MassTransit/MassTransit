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
using System.Threading.Tasks;
using MassTransit;
";

        private readonly string MessageContracts = @"
namespace ConsoleApplication1
{        
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
}
";

        [TestMethod]
        public void WhenPublishTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingInMemory(cfg => { });

            await busControl.Publish<ICreateCommand>(new
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
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenSendTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingInMemory(cfg => { });
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri(""queue:queue_name""));

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
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }


        [TestMethod]
        public void WhenCreateRequestTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{        
    class Program
    {
        static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingInMemory(cfg => { });
            var requestClient = busControl.CreateRequestClient<ICreateCommand>(null);

            var request = requestClient.Create(new
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
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }



        [TestMethod]
        public void WhenReadOnlyListTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
        public void WhenArrayTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IIdentification[]>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
        public void WhenListTypesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<List<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = 0
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type does not map to message contract 'IAddress'. The following properties of the anonymous type are incompatible: Place",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 71, 69)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty,
                Unknown = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type does not map to message contract 'IAddress'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 71, 69)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenReadOnlyListTypesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = 0
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = 0
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 85, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenReadOnlyListTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty,
                    Unknown = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty,
                    Unknown = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 86, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenArrayTypesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IIdentification[]>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = 0
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = 0
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 85, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenArrayTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IIdentification[]>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty,
                    Unknown = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty,
                    Unknown = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 86, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenListTypesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<List<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = 0
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = 0
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 85, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenListTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<List<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty,
                    Unknown = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty,
                    Unknown = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 86, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyIncompatibleAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = 0,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type does not map to message contract 'ICreateCommand'. The following properties of the anonymous type are incompatible: Name",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyIncompatibleWithUnknownPropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                Unknown = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type does not map to message contract 'ICreateCommand'. The following properties of the anonymous type are incompatible: Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyIncompatibleAtNestedTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = 0
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type does not map to message contract 'ICreateCommand'. The following properties of the anonymous type are incompatible: DeliveryAddress.Place",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyIncompatibleWithUnknownPropertyAtNestedTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty,
                    Unknown = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type does not map to message contract 'ICreateCommand'. The following properties of the anonymous type are incompatible: DeliveryAddress.Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyIncompatibleAtNestedArrayTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = 0
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = 0
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
                Message = "Anonymous type does not map to message contract 'ICreateCommand'. The following properties of the anonymous type are incompatible: Documents.Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyIncompatibleWithUnknownPropertyAtNestedArrayTypePropertyAndNoMissingProperties_ShouldHaveDiagnostic()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty,
                        Unknown = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty,
                        Unknown = string.Empty
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
                Message = "Anonymous type does not map to message contract 'ICreateCommand'. The following properties of the anonymous type are incompatible: Documents.Unknown",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'IAddress'. The following properties are missing: Place",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 71, 69)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty
,
                Place = default(string)
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
        public void WhenReadOnlyListTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 84, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
,
Number = default(string) },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
,
Number = default(string) }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WhenArrayTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IIdentification[]>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 84, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IIdentification[]>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
,
Number = default(string) },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
,
Number = default(string) }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WhenListTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<List<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 79, 17)
                        }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 84, 17)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<List<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
,
Number = default(string) },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty
,
Number = default(string) }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: Name",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
,
                Name = default(string)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyCompatibleAndMissingPropertyInNestedType_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: DeliveryAddress.Place",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty
,
                    Place = default(string)
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
        public void WhenCommandTypesAreStructurallyCompatibleAndMissingPropertyInNestedArrayType_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: Documents.Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
,
Number = default(string) },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
,
Number = default(string) }
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyCompatibleAndMissingNestedTypeProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: BillingAddress",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Identifications = identifications,
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
,
                BillingAddress = new
                {
                    Street = default(string),
                    Place = default(string)
                }
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenCommandTypesAreStructurallyCompatibleAndMissingNestedArrayTypeProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: Identifications",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                Name = string.Empty,
                BillingAddress = billingAddress,
                DeliveryAddress = new
                {
                    Street = string.Empty,
                    Place = string.Empty
                },
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty,
                        Number = string.Empty
                    }
                }
,
                Identifications = new[] {
                    new
                    {
                        Type = default(string),
                        IssuingCountry = default(string),
                        Number = default(string)
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
        public void WhenCommandTypesAreStructurallyCompatibleAndMissingMultiplePropertiesAtDifferentNodes_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                DeliveryAddress = new
                {
                    Street = string.Empty
                },
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
                    },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
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
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: Name, BillingAddress, DeliveryAddress.Place, Identifications, Documents.Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 93, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var billingAddress = Activator.CreateInstance<IAddress>(new
            {
                Street = string.Empty,
                Place = string.Empty
            });

            var identifications = Activator.CreateInstance<IReadOnlyList<IIdentification>>(new[]
            {
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                },
                new
                {
                    Type = string.Empty,
                    IssuingCountry = string.Empty,
                    Number = string.Empty
                }
            });

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                StreamId = Guid.Empty,
                DeliveryAddress = new
                {
                    Street = string.Empty
,
                    Place = default(string)
                },
                Documents = new[]
                {
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
,
Number = default(string) },
                    new
                    {
                        Type = string.Empty,
                        IssuingCountry = string.Empty
,
Number = default(string) }
                }
,
                Name = default(string),
                BillingAddress = new
                {
                    Street = default(string),
                    Place = default(string)
                },
                Identifications = new[] {
                    new
                    {
                        Type = default(string),
                        IssuingCountry = default(string),
                        Number = default(string)
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
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyCompatibleAndNoMissingProperties_ShouldHaveNoDiagnostics()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    using System.Linq;

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
        public Guid StreamId { get; }
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
            var request = new CreateRequest();

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                request.StreamId,
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
                }).ToList()
            });
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void WhenAnonymousTypeUsingInferredMemberNamesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    using System.Linq;

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
        public Guid StreamId { get; }
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
            var request = new CreateRequest();

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                request.StreamId,
                request.Name,
                BillingAddress = new
                {
                    request.BillingAddress.Street
                },
                DeliveryAddress = new
                {
                    request.DeliveryAddress.Street,
                    request.DeliveryAddress.Place
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
                }).ToList()
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'ICreateCommand'. The following properties are missing: BillingAddress.Place",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 98, 67)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  MessageContracts + @"
namespace ConsoleApplication1
{
    using System.Linq;

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
        public Guid StreamId { get; }
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
            var request = new CreateRequest();

            var command = Activator.CreateCommand<ICreateCommand>(new
            {
                request.StreamId,
                request.Name,
                BillingAddress = new
                {
                    request.BillingAddress.Street
,
                    Place = default(string)
                },
                DeliveryAddress = new
                {
                    request.DeliveryAddress.Street,
                    request.DeliveryAddress.Place
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
                }).ToList()
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
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
        public IReadOnlyList<string> Codes { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty,
                Number = string.Empty,
                Codes = Array.Empty<string>()
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
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
        public IReadOnlyList<string> Codes { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty,
                Number = string.Empty,
                Codes = Array.Empty<int>()
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Codes",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 47, 76)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenSimpleArrayTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
        public IReadOnlyList<string> Codes { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty,
                Number = string.Empty
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Codes",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 47, 76)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  @"
namespace ConsoleApplication1
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
        public IReadOnlyList<string> Codes { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty,
                Number = string.Empty
,
                Codes = new[] { default(string) }
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
            var test = Usings +  @"
namespace ConsoleApplication1.Messages
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
    }
}
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<Messages.IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty,
                Number = string.Empty
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
            var test = Usings +  @"
namespace ConsoleApplication1.Messages
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
    }
}
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<Messages.IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty,
                Number = 0
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message = "Anonymous type does not map to message contract 'IIdentification'. The following properties of the anonymous type are incompatible: Number",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 48, 85)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenMessageContractHasNamespaceAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  @"
namespace ConsoleApplication1.Messages
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
    }
}
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<Messages.IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 48, 85)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  @"
namespace ConsoleApplication1.Messages
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public string Number { get; }
    }
}
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<Messages.IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty
,
                Number = default(string)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void WhenMessageContractHasNullableAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public int? Number { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message = "Anonymous type is missing properties that are in the message contract 'IIdentification'. The following properties are missing: Number",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 46, 76)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  @"
namespace ConsoleApplication1
{
    public interface IIdentification
    {
        public string Type { get; }
        public string IssuingCountry { get; }
        public int? Number { get; }
    }

    class Program
    {
        static async Task Main()
        {
            var identification = Activator.CreateInstance<IIdentification>(new
            {
                Type = string.Empty,
                IssuingCountry = string.Empty
,
                Number = default(int?)
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
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public Guid StreamId { get; }
    }

    public interface ICreatedNotification : INotification
    {        
    }

    class Program
    {
        static async Task Main()
        {
            var notification = CreateNotification<ICreatedNotification>(Guid.NewGuid());
        }

        private static T CreateNotification<T>(Guid streamId) where T : INotification
        {
            return Activator.CreateInstance<T>(new
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
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public int StreamId { get; }
    }

    public interface ICreatedNotification : INotification
    {        
    }

    class Program
    {
        static async Task Main()
        {
            var notification = CreateNotification<ICreatedNotification>(Guid.NewGuid());
        }

        private static T CreateNotification<T>(Guid streamId) where T : INotification
        {
            return Activator.CreateInstance<T>(new
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
                            new DiagnosticResultLocation("Test0.cs", 53, 48)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            VerifyCSharpFix(test, test);
        }

        [TestMethod]
        public void WhenActivatingGenericContractAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnosticAndCodeFix()
        {
            var test = Usings +  @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public Guid StreamId { get; }
    }

    public interface ICreatedNotification : INotification
    {        
    }

    class Program
    {
        static async Task Main()
        {
            var notification = CreateNotification<ICreatedNotification>(Guid.NewGuid());
        }

        private static T CreateNotification<T>(Guid streamId) where T : INotification
        {
            return Activator.CreateInstance<T>(new
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
                            new DiagnosticResultLocation("Test0.cs", 53, 48)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = Usings +  @"
namespace ConsoleApplication1
{
    public interface INotification
    {
        public Guid StreamId { get; }
    }

    public interface ICreatedNotification : INotification
    {        
    }

    class Program
    {
        static async Task Main()
        {
            var notification = CreateNotification<ICreatedNotification>(Guid.NewGuid());
        }

        private static T CreateNotification<T>(Guid streamId) where T : INotification
        {
            return Activator.CreateInstance<T>(new
            {

                StreamId = default(Guid)
            });
        }
    }
}
";
            VerifyCSharpFix(test, fixtest);
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

namespace MassTransit.DapperIntegration.Tests.ComponentTests
{
    using System;
    using Dapper.Contrib.Extensions;


    [Table("OverrideTable")]
    public class ComplexSaga : ISaga
    {
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

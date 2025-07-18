namespace MassTransit.Persistence.Tests.ComponentTests
{
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("OverrideTable")]
    public class ComplexSaga : ISaga
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public Guid CorrelationId { get; set; }
    }
}

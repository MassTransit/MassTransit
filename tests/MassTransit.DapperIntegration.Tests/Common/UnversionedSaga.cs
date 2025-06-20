namespace MassTransit.DapperIntegration.Tests.Common
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;


    public class UnversionedSaga : ISaga
    {
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        [Column("EarthTrips")]
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string Zip_Code { get; set; }
    }
}

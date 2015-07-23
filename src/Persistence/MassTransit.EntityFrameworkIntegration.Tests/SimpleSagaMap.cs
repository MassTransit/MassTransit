namespace MassTransit.EntityFrameworkIntegration.Tests
{
    public class SimpleSagaMap :
        SagaClassMapping<SimpleSagaEntity>
    {
        public SimpleSagaMap()
        {
            Property(x => x.Name)
                .HasMaxLength(40);
            Property(x => x.Initiated);
            Property(x => x.Observed);
            Property(x => x.Completed);
        }
    }
}

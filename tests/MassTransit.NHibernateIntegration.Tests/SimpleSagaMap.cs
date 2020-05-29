namespace MassTransit.NHibernateIntegration.Tests
{
    using MassTransit.Tests.Saga;


    public class SimpleSagaMap :
        SagaClassMapping<SimpleSaga>
    {
        public SimpleSagaMap()
        {
            Property(x => x.Name, x => x.Length(40));
            Property(x => x.Initiated);
            Property(x => x.Observed);
            Property(x => x.Completed);
        }
    }
}

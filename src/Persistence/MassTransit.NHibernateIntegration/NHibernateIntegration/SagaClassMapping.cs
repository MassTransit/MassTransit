namespace MassTransit.NHibernateIntegration
{
    using NHibernate.Mapping.ByCode;
    using NHibernate.Mapping.ByCode.Conformist;


    /// <summary>
    /// A base class that can be used to define an NHibernate saga map, which
    /// automatically sets the saga to not be lazy and defined the Id property
    /// to have the correct generator of Assigned
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SagaClassMapping<T> :
        ClassMapping<T>
        where T : class, ISaga
    {
        protected SagaClassMapping()
        {
            DynamicInsert(true);
            DynamicUpdate(true);
            Lazy(false);
            Id(x => x.CorrelationId, x => x.Generator(Generators.Assigned));
        }
    }
}

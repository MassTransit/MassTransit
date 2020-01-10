namespace MassTransit.NHibernateIntegration.Saga
{
    using NHibernate;


    public interface NHibernateContext
    {
        ISession Session { get; }
        ITransaction Transaction { get; }
    }
}

namespace MassTransit.NHibernateIntegration
{
    using System.Data.Common;
    using NHibernate.Engine;
    using NHibernate.SqlTypes;


    public interface StateUserTypeConverter
    {
        SqlType[] Types { get; }
        State Get(DbDataReader rs, string[] names, ISessionImplementor session);

        void Set(DbCommand command, object value, int index, ISessionImplementor session);
    }
}

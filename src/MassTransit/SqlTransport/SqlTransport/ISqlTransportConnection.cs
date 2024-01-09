namespace MassTransit.SqlTransport
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;


    public interface ISqlTransportConnection :
        IAsyncDisposable
    {
        IDbConnection Connection { get; }

        Task Open(CancellationToken cancellationToken = default);
        Task Close();
    }
}

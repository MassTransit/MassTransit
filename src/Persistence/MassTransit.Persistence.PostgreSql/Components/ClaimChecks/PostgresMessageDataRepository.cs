namespace MassTransit.MessageData
{
    using System.Data;
    using Npgsql;
    using NpgsqlTypes;
    using Persistence.PostgreSql.Connections;
    using Persistence.PostgreSql.Extensions;

    public class PostgresMessageDataRepository : PessimisticPostgresDatabaseContext<MessageDataSaga>,
        IMessageDataRepository
    {
        readonly Func<DateTimeOffset> _timeProvider;
        readonly string _removeExpired = "DELETE FROM {0} WHERE Expires < @now;";

        public PostgresMessageDataRepository(
            string connectionString,
            string tableName,
            string idColumnName,
            IsolationLevel isolationLevel,
            Func<DateTimeOffset> timeProvider
        )
            : base(connectionString, tableName, idColumnName, isolationLevel)
        {
            _timeProvider = timeProvider;
            _removeExpired = string.Format(_removeExpired, tableName);
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = default)
        {
            var id = ClaimChecks.Unpack(address);
            var model = await LoadAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (model is null)
                throw new KeyNotFoundException($"No claim check available at {address}");

            return model.Data ?? Stream.Null;
        }

        public async Task<Uri> Put(Stream stream, TimeSpan? timeToLive = default, CancellationToken cancellationToken = default)
        {
            var id = NewId.NextSequentialGuid();
            var now = _timeProvider();
            var later = ClaimChecks.GetExpiration(now, timeToLive);

            if (now > later)
                throw new InvalidOperationException("TTL has already expired");

            var model = new MessageDataSaga
            {
                CorrelationId = id,
                Created = now,
                Expires = later,
                Data = stream
            };

            await InsertAsync(model, cancellationToken)
                .ConfigureAwait(false);

            await CommitAsync(cancellationToken)
                .ConfigureAwait(false);

            return ClaimChecks.Pack(id);
        }

        public async Task<int> CleanupAsync(CancellationToken cancellationToken = default)
        {
            var now = _timeProvider();

            var changed = await ExecuteAsync(_removeExpired, new { now }, cancellationToken)
                .ConfigureAwait(false);

            await CommitAsync(cancellationToken)
                .ConfigureAwait(false);

            return changed;
        }

        protected override Func<IDataReader, MessageDataSaga> CreateReaderAdapter() => MapFrom;

        MessageDataSaga MapFrom(IDataReader reader)
        {
            if (reader is not NpgsqlDataReader r)
                throw new InvalidOperationException("Invalid data reader");

            var stream = new MemoryStream();
            r.GetStream("Data").CopyTo(stream, 81920);
            stream.Position = 0;

            return new MessageDataSaga
            {
                CorrelationId = r.GetGuid(IdColumnName),
                Created = r.GetDateTime("Created"),
                Expires = r.GetDateTime("Expires"),
                Data = stream
            };
        }

        protected override Action<object?, NpgsqlParameterCollection> CreateWriterAdapter() => MapTo;

        static void MapTo(object? parameters, NpgsqlParameterCollection collection)
        {
            if (parameters is MessageDataSaga { Data: not null } s)
            {
                collection.Add("@correlationid", NpgsqlDbType.Uuid).Value = s.CorrelationId;
                collection.Add("@created", NpgsqlDbType.TimestampTz).Value = s.Created;
                collection.Add("@expires", NpgsqlDbType.TimestampTz).Value = s.Expires;
                collection.Add("@data", NpgsqlDbType.Bytea, -1).Value = s.Data;
                return;
            }

            AssignParameters(parameters, collection);
        }
    }
}

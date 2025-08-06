namespace MassTransit.Persistence.Integration.Saga
{
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;


    /// <summary>
    /// Handles the base logic for connectors to plug into.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public abstract class SagaDatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
#if NET8_0_OR_GREATER
        protected static readonly ValueTask CompletedTask = ValueTask.CompletedTask;
#else
        protected static readonly ValueTask CompletedTask = default;
#endif

        protected readonly List<ModelPropertyMapping> Mappings = new();
        protected readonly Type ModelType = typeof(TSaga);

        public async Task<TSaga?> LoadAsync(Guid correlationId, CancellationToken cancellationToken)
        {
            var sql = BuildLoadSql();

            LogContext.Debug?.Log("Loading: {sql}", sql, correlationId);

            var results = ReadAsync(
                sql,
                new { correlationId },
                cancellationToken
            ).ConfigureAwait(false);

            // intentionally returning inside the foreach,
            // since we only need at most one result
            await foreach (var result in results)
            {
                LogContext.Debug?.Log("Instance found for {id}", correlationId);

                return result;
            }

            LogContext.Debug?.Log("No instance found for {id}", correlationId);

            return null;
        }

        public async IAsyncEnumerable<TSaga> QueryAsync(Expression<Func<TSaga, bool>> filterExpression,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object?>();
            var sql = BuildQuerySql(filterExpression, (k, v) => parameters[k] = v);

            LogContext.Debug?.Log("Querying: {sql}");

            var results = ReadAsync(
                sql,
                parameters,
                cancellationToken
            ).ConfigureAwait(false);

            var instances = 0;
            await foreach (var result in results)
            {
                // have to increment instead of .Length / .Count, because
                // this IAsyncEnumerable is NOT backed by a collection type
                instances++;
                yield return result;
            }

            LogContext.Debug?.Log("Loaded {count} instances", instances);
        }

        public async Task InsertAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var sql = BuildInsertSql();

            LogContext.Debug?.Log("Inserting: {sql}", sql);

            var rows = await ExecuteAsync(
                sql,
                instance,
                cancellationToken
            ).ConfigureAwait(false);

            if (rows == 0)
                throw new SagaConcurrencyException("Saga Insert failed", instance);

            LogContext.Debug?.Log("Instance added: {id}", instance.CorrelationId);
        }

        public async Task UpdateAsync(TSaga instance, CancellationToken cancellationToken = default)
        {
            var sql = BuildUpdateSql();

            LogContext.Debug?.Log("Updating: {sql}", sql);

            var rows = await ExecuteAsync(
                sql,
                instance,
                cancellationToken
            ).ConfigureAwait(false);

            if (rows == 0)
                throw new SagaConcurrencyException("Saga Update failed", instance);

            LogContext.Debug?.Log("Instance updated: {id}", instance.CorrelationId);
        }

        public async Task DeleteAsync(TSaga instance, CancellationToken cancellationToken)
        {
            var sql = BuildDeleteSql();

            LogContext.Debug?.Log("Deleting: {sql}", sql);

            var rows = await ExecuteAsync(
                sql,
                instance,
                cancellationToken
            ).ConfigureAwait(false);

            if (rows == 0)
                throw new SagaConcurrencyException("Saga Delete failed", instance);

            LogContext.Debug?.Log("Instance deleted: {id}", instance.CorrelationId);
        }

        protected abstract IAsyncEnumerable<TSaga> ReadAsync(string sql, object? parameters, CancellationToken cancellationToken);

        protected abstract Task<int> ExecuteAsync(string sql, object? parameters, CancellationToken cancellationToken);

        protected internal abstract string BuildLoadSql();

        protected internal abstract string BuildQuerySql(Expression<Func<TSaga, bool>> filterExpression, Action<string, object?> parameterCallback);

        protected internal abstract string BuildInsertSql();

        protected internal abstract string BuildUpdateSql();

        protected internal abstract string BuildDeleteSql();

        /// <summary>
        /// Maps nested objects in the saga to prefixed columns.  The default SQL builder uses the
        /// name of the property, and an embedded object may end up with the wrong identifier in
        /// the generated SQL.  This solves the problem of `saga.User.Id == 5` being interpreted as
        /// "WHERE Id = 5" in SQL, when it should be something like "WHERE User_Id = 5".
        /// </summary>
        /// <example>
        /// <code>
        /// class OrderSaga
        /// {
        ///   public Guid CorrelationId { get; set; }
        ///   public UserModel? User { get; set; }
        /// }
        ///
        /// class UserModel
        /// {
        ///   public int Id { get; set; }
        ///   public string Name { get; set; }
        /// }
        ///
        /// CREATE TABLE Orders
        /// (
        ///    CorrelationId UNIQUEIDENTIFIER,
        ///    User_Id INT,
        ///    User_Name VARCHAR(50)
        /// );
        ///
        /// MapPrefix(m => m.User, "User_");
        ///
        /// // with this correlation expression:
        /// CorrelateBy((s, c) => s.User_Id == c.Message.UserId);
        ///
        /// // builds this SQL:
        /// // WHERE User_Id = @userid
        /// </code>
        /// </example>
        protected void MapPrefix<TProperty>(Expression<Func<TSaga, TProperty>> mappingExpression, string? prefixName = null)
        {
            MapCore(mappingExpression, prefixName, PropertyMappingBehavior.PrefixOnly);
        }

        /// <summary>
        /// Similar to <seealso cref="MapPrefix{TProperty}"/>, MapProperty will specify an explicit
        /// 1:1 mapping of property to SQL.  Prefixes are used for all properties of a nested model,
        /// whereas Property refers to a single property. 
        /// </summary>
        /// <remarks>If you're getting to these, you should probably just use EF if you can.</remarks>
        protected void MapProperty<TProperty>(Expression<Func<TSaga, TProperty>> mappingExpression, string targetName)
        {
            MapCore(mappingExpression, targetName, PropertyMappingBehavior.RequireExact);
        }

        /// <summary>
        /// Tells the underlying SQL builder to ignore this model property entirely.  Prevent it from
        /// being mapped to/from.
        /// </summary>
        protected void Ignore<TProperty>(Expression<Func<TSaga, TProperty>> mappingExpression)
        {
            MapCore(mappingExpression, null, PropertyMappingBehavior.IgnoreProperty);
        }

        void MapCore<TModel, TProperty>(Expression<Func<TModel, TProperty>> mappingExpression, string? name, PropertyMappingBehavior behavior)
        {
            if (mappingExpression.NodeType != ExpressionType.Lambda)
                throw new InvalidOperationException("Expression must be a lambda");

            var body = mappingExpression.Body as MemberExpression;
            if (body is null)
                throw new InvalidOperationException("Expression must only be a property (x => x.Foo.Bar)");

            Mappings.Add(new ModelPropertyMapping
            {
                Property = body,
                Name = name ?? body.Member.Name,
                Behavior = behavior
            });
        }
    }
}

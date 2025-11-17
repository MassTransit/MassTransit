namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Options;
    using Middleware;


    public class EntityFrameworkOutboxContextFactory<TDbContext> :
        IOutboxContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        readonly TDbContext _dbContext;
        readonly IsolationLevel _isolationLevel;
        readonly ILockStatementProvider _lockStatementProvider;
        readonly IServiceProvider _provider;
        string _lockStatement;

        public EntityFrameworkOutboxContextFactory(TDbContext dbContext, IServiceProvider provider, IOptions<EntityFrameworkOutboxOptions<TDbContext>> options)
        {
            _dbContext = dbContext;
            _provider = provider;
            _lockStatementProvider = options.Value.LockStatementProvider;
            _isolationLevel = options.Value.IsolationLevel;
        }

        public async Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
            where T : class
        {
            var messageId = context.GetOriginalMessageId() ?? throw new MessageException(typeof(T), "MessageId required to use the outbox");

            _lockStatement ??= _lockStatementProvider.GetRowLockStatement<InboxState>(_dbContext, nameof(InboxState.MessageId), nameof(InboxState.ConsumerId));

            async Task<bool> Execute()
            {
                var lockId = NewId.NextGuid();

                var timer = Stopwatch.StartNew();

                await using var transaction = await _dbContext.Database.BeginTransactionAsync(_isolationLevel, context.CancellationToken)
                    .ConfigureAwait(false);

                try
                {
                    List<InboxState> inboxStateList = await _dbContext.Set<InboxState>()
                        .FromSqlRaw(_lockStatement, messageId, options.ConsumerId)
                        .AsTracking()
                        .ToListAsync(context.CancellationToken).ConfigureAwait(false);
                    var inboxState = inboxStateList.SingleOrDefault();

                    bool continueProcessing;

                    if (inboxState == null)
                    {
                        inboxState = new InboxState
                        {
                            MessageId = messageId,
                            ConsumerId = options.ConsumerId,
                            Received = DateTime.UtcNow,
                            LockId = lockId,
                            ReceiveCount = 0
                        };

                        await _dbContext.AddAsync(inboxState).ConfigureAwait(false);
                        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                        continueProcessing = true;
                    }
                    else
                    {
                        inboxState.LockId = lockId;
                        inboxState.ReceiveCount++;

                        _dbContext.Update(inboxState);
                        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

                        var outboxContext = new DbContextOutboxConsumeContext<TDbContext, T>(context, options, _provider, _dbContext, transaction, inboxState);

                        await next.Send(outboxContext).ConfigureAwait(false);

                        try
                        {
                            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            await context.NotifyFaulted(timer.Elapsed, TypeCache<T>.ShortName, exception).ConfigureAwait(false);

                            throw;
                        }

                        continueProcessing = outboxContext.ContinueProcessing;
                    }

                    try
                    {
                        await transaction.CommitAsync(context.CancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        await context.NotifyFaulted(timer.Elapsed, TypeCache<T>.ShortName, exception).ConfigureAwait(false);

                        throw;
                    }

                    return continueProcessing;
                }
                catch (Exception)
                {
                    try
                    {
                        await transaction.RollbackAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        //
                    }

                    throw;
                }
            }

            var continueProcessing = true;
            while (continueProcessing)
            {
                var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
                if (executionStrategy is ExecutionStrategy)
                    continueProcessing = await executionStrategy.ExecuteAsync(() => Execute()).ConfigureAwait(false);
                else
                    continueProcessing = await Execute().ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outboxContextFactory");
            scope.Add("provider", "entityFrameworkCore");
        }
    }
}

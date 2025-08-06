namespace MassTransit.Persistence.Tests.ComponentTests
{
    using System.Data;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MySqlConnector;
    using NUnit.Framework;
    using Persistence.MySql.Configuration;
    using Persistence.MySql.Connections;
    using Persistence.MySql.Extensions;


    public class Configurator_Documentation_Tests
    {
        [Test]
        public void Registration_looks_correct()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(bus =>
            {
                bus.AddSagaStateMachine<OrderStateMachine, OrderSaga>()
                    .CustomRepository(conf => conf.UsingMySql(opt => opt
                        .SetConnectionString("my connection string")
                        .SetOptimisticConcurrency()
                    ));

                bus.AddJobSagaStateMachines()
                    .CustomRepository(conf => conf.UsingMySql(
                        opt => opt.SetConnectionString("my connection string")
                    ));

                bus.UsingInMemory((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }


    public class OrderSaga : SagaStateMachineInstance
    {
        public byte[] RowVersion { get; set; }
        public int CurrentState { get; set; }
        public string OrderNumber { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
        public Guid CorrelationId { get; set; }
    }


    public record OrderItem(Guid ItemId, int Quantity, decimal ItemPrice);


    public class OrderStateMachine : MassTransitStateMachine<OrderSaga>
    {
    }


    public class MySqlOrderSagaRepository : OptimisticMySqlDatabaseContext<OrderSaga>
    {
        public MySqlOrderSagaRepository(string connectionString)
            // base constructor shows setting the table, id, and version column/property names
            : base(connectionString, "OrderSagas", "CorrelationId", "RowVersion", "RowVersion")
        {
            MapProperty(saga => saga.Items, "ItemsJson");
        }

        protected override Func<IDataReader, OrderSaga> CreateReaderAdapter()
        {
            return MapFromReader;
        }

        static OrderSaga MapFromReader(IDataReader reader)
        {
            var r = (MySqlDataReader)reader;
            return new OrderSaga
            {
                CorrelationId = r.GetGuid("CorrelationId"),
                CurrentState = r.GetInt32("CurrentState"),
                OrderNumber = r.GetString("OrderNumber"),
                CustomerId = r.GetGuidOrNull("CustomerId"),
                TotalAmount = r.GetDecimal("TotalAmount"),
                Items = r.FromJson<List<OrderItem>>("ItemsJson") ?? [],
                CreatedOn = r.GetDateTime("CreatedOn"),
                UpdatedOn = r.GetDateTime("UpdatedOn"),
                RowVersion = r.GetFieldValue<byte[]>("RowVersion")
            };
        }

        protected override Action<object?, MySqlParameterCollection> CreateWriterAdapter()
        {
            return MapToParameters;
        }

        static void MapToParameters(object? input, MySqlParameterCollection parameters)
        {
            if (input is OrderSaga saga)
            {
                parameters.Add("@correlationid", MySqlDbType.Guid).Value = saga.CorrelationId;
                parameters.Add("@currentstate", MySqlDbType.Int32).Value = saga.CurrentState;
                parameters.Add("@ordernumber", MySqlDbType.VarChar, 50).Value = saga.OrderNumber;
                parameters.Add("@customerid", MySqlDbType.Guid).Value = saga.CustomerId.OrDbNull();
                parameters.Add("@totalamount", MySqlDbType.Decimal).Value = saga.TotalAmount;
                parameters.Add("@items", MySqlDbType.Text).Value = saga.Items.ToJson().OrDbNull();
                parameters.Add("@rowversion", MySqlDbType.Timestamp).Value = saga.RowVersion;
                return;
            }

            // Fallback for query parameters
            AssignParameters(input, parameters);
        }
    }
}

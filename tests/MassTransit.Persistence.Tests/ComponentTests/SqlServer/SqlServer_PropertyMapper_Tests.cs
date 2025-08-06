//using MassTransit.Persistence.Integration.Saga;
//using MassTransit.Persistence.SqlServer.Connections;
//using NUnit.Framework;

//namespace MassTransit.Persistence.Tests.ComponentTests.SqlServer
//{
//    using System.Data;
//    using System.Linq.Expressions;
//    using Integration.SqlBuilders;

//    [TestFixture]
//    public class SqlServer_PropertyMapper_Tests : SqlServer_Tests
//    {
//        protected SagaDatabaseContext<OrderSaga> Subject = new SqlServerOrderSagaRepository("");

//        [Test]
//        public void Properties_can_be_renamed_via_mapping()
//        {
//            var expected = @"UPDATE OrderSagas SET [CurrentState] = @currentstate, [OrderNumber] = @ordernumber, [CustomerId] = @customerid, [TotalAmount] = @totalamount, [CreatedOn] = @createdon, [UpdatedOn] = @updatedon, [ItemsJson] = @items WHERE [CorrelationId] = @correlationid AND [RowVersion] = @rowversion";
//            var actual = Subject.BuildUpdateSql();

//            Assert.That(actual, Is.EqualTo(expected));
//        }
//    }
//}

///*
//   public Guid CorrelationId { get; set; }
//   public byte[] RowVersion { get; set; }
//   public int CurrentState { get; set; }
//   public string OrderNumber { get; set; }
//   public Guid? CustomerId { get; set; }
//   public decimal TotalAmount { get; set; }
//   public DateTime CreatedOn { get; set; }
//   public DateTime UpdatedOn { get; set; }

//   public ICollection<OrderItem> Items { get; set; } = [];
//*/


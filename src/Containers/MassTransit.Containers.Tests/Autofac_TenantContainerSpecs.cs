// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Containers.Tests
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Autofac;
    using AutofacIntegration;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class Using_a_tenant_with_a_child_container :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_create_separate_scope_for_each_customer()
        {
            await InputQueueSendEndpoint.Send(new UpdateOrder {CustomerId = "A", OrderId = "123", OrderStatus = "Pending"});
            await InputQueueSendEndpoint.Send(new UpdateOrder {CustomerId = "B", OrderId = "456", OrderStatus = "Pending"});
            await InputQueueSendEndpoint.Send(new UpdateOrder {CustomerId = "A", OrderId = "456", OrderStatus = "Processed"});
            await InputQueueSendEndpoint.Send(new UpdateOrder {CustomerId = "C", OrderId = "789", OrderStatus = "Pending"});
            await InputQueueSendEndpoint.Send(new UpdateOrder {CustomerId = "A", OrderId = "123", OrderStatus = "Processed"});

            await _updated;

            var orderInventory = _container.Resolve<ILifetimeScopeRegistry<string>>().GetLifetimeScope("A").Resolve<IOrderInventory>();

            Assert.AreEqual(2, orderInventory.Count);

             orderInventory = _container.Resolve<ILifetimeScopeRegistry<string>>().GetLifetimeScope("B").Resolve<IOrderInventory>();
            Assert.AreEqual(1, orderInventory.Count);
        }

        IContainer _container;
        Task<ConsumeContext<OrderUpdated>> _updated;
        const string CustomerScopeTag = "Customer";


        class ConsumerModule :
            Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<OrderUpdateConsumer>();
                builder.RegisterType<OrderInventory>()
                    .As<IOrderInventory>()
                    .InstancePerMatchingLifetimeScope(CustomerScopeTag);

                builder.RegisterLifetimeScopeRegistry<string>(CustomerScopeTag);

                builder.RegisterLifetimeScopeIdAccessor<UpdateOrder, string>(x => x.CustomerId);
            }
        }


        interface IOrderInventory
        {
            int Count { get; }
            void UpdateOrder(string orderId, string orderStatus);
        }


        class OrderInventory :
            IOrderInventory
        {
            readonly ConcurrentDictionary<string, string> _orders;

            public OrderInventory()
            {
                _orders = new ConcurrentDictionary<string, string>();
            }

            void IOrderInventory.UpdateOrder(string orderId, string orderStatus)
            {
                _orders.AddOrUpdate(orderId, n => orderStatus, (n, u) => orderStatus);
            }

            public int Count
            {
                get { return _orders.Count; }
            }
        }


        public class UpdateOrder
        {
            public string CustomerId { get; set; }
            public string OrderId { get; set; }
            public string OrderStatus { get; set; }
        }


        public class OrderUpdated
        {
            public string OrderId { get; set; }
            public string OrderStatus { get; set; }
        }


        class OrderUpdateConsumer :
            IConsumer<UpdateOrder>

        {
            readonly IOrderInventory _inventory;

            public OrderUpdateConsumer(IOrderInventory inventory)
            {
                _inventory = inventory;
            }

            public Task Consume(ConsumeContext<UpdateOrder> context)
            {
                _inventory.UpdateOrder(context.Message.OrderId, context.Message.OrderStatus);

                context.Publish(new OrderUpdated {OrderId = context.Message.OrderId, OrderStatus = context.Message.OrderStatus});

                return TaskUtil.Completed;
            }
        }


        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseConcurrencyLimit(1);

            var builder = new ContainerBuilder();

            builder.RegisterModule<ConsumerModule>();

            _container = builder.Build();

            // uses the registry which we should resolve in the method
            configurator.ConsumerInScope<OrderUpdateConsumer, string>(_container);

            _updated = Handled<OrderUpdated>(configurator, context => context.Message.OrderId == "123" && context.Message.OrderStatus == "Processed");
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            _container.Dispose();
        }
    }
}
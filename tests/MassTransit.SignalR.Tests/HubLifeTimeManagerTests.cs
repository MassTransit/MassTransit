namespace MassTransit.SignalR.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using NUnit.Framework;
    using OfficialFramework;


    public class HubLifeTimeManagerTests : SingleScaleoutBackplaneTestFixture<MyHub>
    {
        async Task AssertMessageAsync(TestClient client)
        {
            var message = await client.ReadAsync().OrTimeout() as InvocationMessage;
            Assert.That(message, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(message.Target, Is.EqualTo("Hello"));
                Assert.That(message.Arguments, Has.Length.EqualTo(1));
            });
            Assert.That(message.Arguments[0].ToString(), Is.EqualTo("World"));
        }

        static async Task AssertNoMessageAsync(TestClient client, int milliseconds = 1000)
        {
            Assert.ThrowsAsync<TimeoutException>(async () => await client.ReadAsync().OrTimeout(milliseconds));
        }

        [Test]
        public async Task SendAllAsyncWritesToAllConnectionsOutput()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.SendAllAsync("Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                Assert.That(BackplaneHarness.All.Consumed.Select<All<MyHub>>().Any(), Is.True);

                var message = client1.TryRead() as InvocationMessage;
                Assert.That(message, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(message.Target, Is.EqualTo("Hello"));
                    Assert.That(message.Arguments, Has.Length.EqualTo(1));
                });
                Assert.That(message.Arguments[0].ToString(), Is.EqualTo("World"));

                message = client2.TryRead() as InvocationMessage;
                Assert.That(message, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(message.Target, Is.EqualTo("Hello"));
                    Assert.That(message.Arguments, Has.Length.EqualTo(1));
                });
                Assert.That(message.Arguments[0].ToString(), Is.EqualTo("World"));
            }
        }

        [Test]
        public async Task SendAllAsyncDoesNotWriteToDisconnectedConnectionsOutput()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.OnDisconnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.SendAllAsync("Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                Assert.That(BackplaneHarness.All.Consumed.Select<All<MyHub>>().Any(), Is.True);

                var message = client1.TryRead() as InvocationMessage;
                Assert.That(message, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(message.Target, Is.EqualTo("Hello"));
                    Assert.That(message.Arguments, Has.Length.EqualTo(1));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(message.Arguments[0].ToString(), Is.EqualTo("World"));

                    Assert.That(client2.TryRead(), Is.Null);
                });
            }
        }

        [Test]
        public async Task SendGroupAsyncWritesToAllConnectionsInGroupOutput()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.AddToGroupAsync(connection1.ConnectionId, "group").OrTimeout(Harness.TestTimeout);

                // Because connection is local, should not have any GroupManagement
                //Assert.IsFalse(backplaneConsumers.GroupManagementConsumer.Consumed.Select<GroupManagement<MyHub>>().Any());

                await manager.SendGroupAsync("group", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                Assert.That(BackplaneHarness.Group.Consumed.Select<Group<MyHub>>().Any(), Is.True);

                var message = client1.TryRead() as InvocationMessage;
                Assert.That(message, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(message.Target, Is.EqualTo("Hello"));
                    Assert.That(message.Arguments, Has.Length.EqualTo(1));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(message.Arguments[0].ToString(), Is.EqualTo("World"));

                    Assert.That(client2.TryRead(), Is.Null);
                });
            }
        }

    #region From Scaleout

        [Test]
        public async Task DisconnectConnectionRemovesConnectionFromGroup()
        {
            using (var client = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection = HubConnectionContextUtils.Create(client.Connection);

                await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await manager.AddToGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);

                await Task.Delay(2000);

                await manager.OnDisconnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await Task.Delay(2000);

                await manager.SendGroupAsync("name", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                await Task.Delay(2000);

                Assert.That(client.TryRead(), Is.Null);
            }
        }

        [Test]
        public async Task RemoveGroupFromLocalConnectionNotInGroupDoesNothing()
        {
            using (var client = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection = HubConnectionContextUtils.Create(client.Connection);

                await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await manager.RemoveFromGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);

                Assert.That(BackplaneHarness.GroupManagement.Consumed.Select<GroupManagement<MyHub>>()
                    .Any(), Is.False); // Should not have published, because connection was local
            }
        }

        [Test]
        public async Task AddGroupAsyncForLocalConnectionAlreadyInGroupDoesNothing()
        {
            using (var client = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection = HubConnectionContextUtils.Create(client.Connection);

                await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await manager.AddToGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);
                await manager.AddToGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);

                await manager.SendGroupAsync("name", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                await AssertMessageAsync(client);
                Assert.That(client.TryRead(), Is.Null);
            }
        }

        [Test]
        public async Task WritingToGroupWithOneConnectionFailingSecondConnectionStillReceivesMessage()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                // Force an exception when writing to connection
                var connectionMock = HubConnectionContextUtils.CreateMock(client1.Connection);

                var connection1 = connectionMock;
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.AddToGroupAsync(connection1.ConnectionId, "group").OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);
                await manager.AddToGroupAsync(connection2.ConnectionId, "group").OrTimeout(Harness.TestTimeout);

                await manager.SendGroupAsync("group", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
                // connection1 will throw when receiving a group message, we are making sure other connections
                // are not affected by another connection throwing
                await AssertMessageAsync(client2);

                // Repeat to check that group can still be sent to
                await manager.SendGroupAsync("group", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
                await AssertMessageAsync(client2);
            }
        }

        [Test]
        public async Task InvokeUserSendsToAllConnectionsForUser()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            using (var client3 = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection, userIdentifier: "userA");
                var connection2 = HubConnectionContextUtils.Create(client2.Connection, userIdentifier: "userA");
                var connection3 = HubConnectionContextUtils.Create(client3.Connection, userIdentifier: "userB");

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection3).OrTimeout(Harness.TestTimeout);

                await manager.SendUserAsync("userA", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
                await AssertMessageAsync(client1);
                await AssertMessageAsync(client2);
            }
        }

        [Test]
        public async Task ConnectionShouldReceiveMessage()
        {
            using var client = new TestClient();

            MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

            var connection = HubConnectionContextUtils.Create(client.Connection);

            await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

            await manager.SendConnectionAsync(connection.ConnectionId, "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
            await AssertMessageAsync(client);
        }

        [Test]
        public async Task ConnectionShouldNotBeReceivedIfConnectionIdCasingIsOff()
        {
            using var client = new TestClient();

            MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

            var connection = HubConnectionContextUtils.Create(client.Connection);

            await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

            await manager.SendConnectionAsync(connection.ConnectionId.ToUpper(), "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
            await AssertNoMessageAsync(client);
        }

        [Test]
        public async Task ExcludedConnectionShouldNotReceiveSendAllMessage()
        {
            using var client = new TestClient();

            MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

            var connection = HubConnectionContextUtils.Create(client.Connection);

            await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

            await manager.SendAllExceptAsync("Hello", new object[] { "World" }, new[] { connection.ConnectionId }).OrTimeout(Harness.TestTimeout);
            await AssertNoMessageAsync(client);
        }

        [Test]
        public async Task ExcludedConnectionShouldReceiveSendAllMessageWhenCasingIsOff()
        {
            using var client = new TestClient();

            MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

            var connection = HubConnectionContextUtils.Create(client.Connection);

            await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

            await manager.SendAllExceptAsync("Hello", new object[] { "World" }, new[] { connection.ConnectionId.ToUpper() }).OrTimeout(Harness.TestTimeout);
            await AssertMessageAsync(client);
        }

        [Test]
        public async Task ExcludedConnectionShouldNotReceiveSendGroupMessage()
        {
            const string groupName = "group";

            using var client = new TestClient();

            MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

            var connection = HubConnectionContextUtils.Create(client.Connection);

            await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

            await manager.AddToGroupAsync(connection.ConnectionId, groupName).OrTimeout(Harness.TestTimeout);

            await manager.SendGroupExceptAsync(groupName, "Hello", new object[] { "World" }, new[] { connection.ConnectionId }).OrTimeout(Harness.TestTimeout);
            await AssertNoMessageAsync(client);
        }

        [Test]
        public async Task ExcludedConnectionShouldReceiveSendGroupMessageWhenCasingIsOff()
        {
            const string groupName = "group";

            using var client = new TestClient();

            MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

            var connection = HubConnectionContextUtils.Create(client.Connection);

            await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

            await manager.AddToGroupAsync(connection.ConnectionId, groupName).OrTimeout(Harness.TestTimeout);

            await manager.SendGroupExceptAsync(groupName, "Hello", new object[] { "World" }, new[] { connection.ConnectionId.ToUpper() })
                .OrTimeout(Harness.TestTimeout);
            await AssertMessageAsync(client);
        }

        [Test]
        public async Task StillSubscribedToUserAfterOneOfMultipleConnectionsAssociatedWithUserDisconnects()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            using (var client3 = new TestClient())
            {
                MassTransitHubLifetimeManager<MyHub> manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection, userIdentifier: "userA");
                var connection2 = HubConnectionContextUtils.Create(client2.Connection, userIdentifier: "userA");
                var connection3 = HubConnectionContextUtils.Create(client3.Connection, userIdentifier: "userB");

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection3).OrTimeout(Harness.TestTimeout);

                await manager.SendUserAsync("userA", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
                await AssertMessageAsync(client1);
                await AssertMessageAsync(client2);

                // Disconnect one connection for the user
                await manager.OnDisconnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.SendUserAsync("userA", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);
                await AssertMessageAsync(client2);
            }
        }

    #endregion From Scaleout

        //[Test]
        //public async Task SendGroupExceptAsyncDoesNotWriteToExcludedConnections()
        //{
        //    var components = CreateNewHubLifetimeManager();
        //    await _harness.Start();

        //    using (var client1 = new TestClient())
        //    using (var client2 = new TestClient())
        //    {

        //        var manager = components.HubLifetimeManager;
        //        var connection1 = HubConnectionContextUtils.Create(client1.Connection);
        //        var connection2 = HubConnectionContextUtils.Create(client2.Connection);

        //        await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
        //        await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

        //        await manager.AddToGroupAsync(connection1.ConnectionId, "group1").OrTimeout(Harness.TestTimeout);
        //        await manager.AddToGroupAsync(connection2.ConnectionId, "group1").OrTimeout(Harness.TestTimeout);

        //        await manager.SendGroupExceptAsync("group1", "Hello", new object[] { "World" }, new[] { connection2.ConnectionId }).OrTimeout(Harness.TestTimeout);

        //        var message = client1.TryRead() as InvocationMessage;
        //        Assert.NotNull(message);
        //        Assert.Equals("Hello", message.Target);
        //        Assert.Equals(1, message.Arguments.Length);
        //        Assert.Equals("World", (string)message.Arguments[0]);

        //        Assert.Null(client2.TryRead());
        //    }
        //}

        //[Test]
        //public async Task SendConnectionAsyncWritesToConnectionOutput()
        //{
        //    var components = CreateNewHubLifetimeManager();
        //    await _harness.Start();

        //    using (var client = new TestClient())
        //    {
        //        var manager = components.HubLifetimeManager;
        //        var connection = HubConnectionContextUtils.Create(client.Connection);

        //        await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

        //        await manager.SendConnectionAsync(connection.ConnectionId, "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

        //        var message = client.TryRead() as InvocationMessage;
        //        Assert.NotNull(message);
        //        Assert.Equals("Hello", message.Target);
        //        Assert.Equals(1, message.Arguments.Length);
        //        Assert.Equals("World", (string)message.Arguments[0]);
        //    }
        //}
    }


    //public class TestConsumer : IConsumer<All<MyHub>>
    //{
    //    public Task Consume(ConsumeContext<All<MyHub>> context)
    //    {
    //        var a = true;

    //        return Task.FromResult(0);
    //    }
    //}
}

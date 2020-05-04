namespace MassTransit.SignalR.Tests
{
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
            Assert.NotNull(message);
            Assert.AreEqual("Hello", message.Target);
            Assert.AreEqual(1, message.Arguments.Length);
            Assert.AreEqual("World", (string)message.Arguments[0]);
        }

        [Test]
        public async Task SendAllAsyncWritesToAllConnectionsOutput()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.SendAllAsync("Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                Assert.IsTrue(BackplaneHarness.All.Consumed.Select<All<MyHub>>().Any());

                var message = client1.TryRead() as InvocationMessage;
                Assert.NotNull(message);
                Assert.AreEqual("Hello", message.Target);
                Assert.AreEqual(1, message.Arguments.Length);
                Assert.AreEqual("World", (string)message.Arguments[0]);

                message = client2.TryRead() as InvocationMessage;
                Assert.NotNull(message);
                Assert.AreEqual("Hello", message.Target);
                Assert.AreEqual(1, message.Arguments.Length);
                Assert.AreEqual("World", (string)message.Arguments[0]);
            }
        }

        [Test]
        public async Task SendAllAsyncDoesNotWriteToDisconnectedConnectionsOutput()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.OnDisconnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.SendAllAsync("Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                Assert.IsTrue(BackplaneHarness.All.Consumed.Select<All<MyHub>>().Any());

                var message = client1.TryRead() as InvocationMessage;
                Assert.NotNull(message);
                Assert.AreEqual("Hello", message.Target);
                Assert.AreEqual(1, message.Arguments.Length);
                Assert.AreEqual("World", (string)message.Arguments[0]);

                Assert.Null(client2.TryRead());
            }
        }

        [Test]
        public async Task SendGroupAsyncWritesToAllConnectionsInGroupOutput()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

                var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                await manager.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                await manager.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                await manager.AddToGroupAsync(connection1.ConnectionId, "group").OrTimeout(Harness.TestTimeout);

                // Because connection is local, should not have any GroupManagement
                //Assert.IsFalse(backplaneConsumers.GroupManagementConsumer.Consumed.Select<GroupManagement<MyHub>>().Any());

                await manager.SendGroupAsync("group", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                Assert.IsTrue(BackplaneHarness.Group.Consumed.Select<Group<MyHub>>().Any());

                var message = client1.TryRead() as InvocationMessage;
                Assert.NotNull(message);
                Assert.AreEqual("Hello", message.Target);
                Assert.AreEqual(1, message.Arguments.Length);
                Assert.AreEqual("World", (string)message.Arguments[0]);

                Assert.Null(client2.TryRead());
            }
        }

        #region From Scaleout
        [Test]
        public async Task DisconnectConnectionRemovesConnectionFromGroup()
        {
            using (var client = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

                var connection = HubConnectionContextUtils.Create(client.Connection);

                await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await manager.AddToGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);

                await Task.Delay(2000);

                await manager.OnDisconnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await Task.Delay(2000);

                await manager.SendGroupAsync("name", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                await Task.Delay(2000);

                Assert.Null(client.TryRead());
            }
        }

        [Test]
        public async Task RemoveGroupFromLocalConnectionNotInGroupDoesNothing()
        {
            using (var client = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

                var connection = HubConnectionContextUtils.Create(client.Connection);

                await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await manager.RemoveFromGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);

                Assert.IsFalse(BackplaneHarness.GroupManagement.Consumed.Select<GroupManagement<MyHub>>().Any()); // Should not have published, because connection was local
            }
        }

        [Test]
        public async Task AddGroupAsyncForLocalConnectionAlreadyInGroupDoesNothing()
        {
            using (var client = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

                var connection = HubConnectionContextUtils.Create(client.Connection);

                await manager.OnConnectedAsync(connection).OrTimeout(Harness.TestTimeout);

                await manager.AddToGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);
                await manager.AddToGroupAsync(connection.ConnectionId, "name").OrTimeout(Harness.TestTimeout);

                await manager.SendGroupAsync("name", "Hello", new object[] { "World" }).OrTimeout(Harness.TestTimeout);

                await AssertMessageAsync(client);
                Assert.Null(client.TryRead());
            }
        }

        [Test]
        public async Task WritingToGroupWithOneConnectionFailingSecondConnectionStillReceivesMessage()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

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
                var manager = BackplaneHarness.HubLifetimeManager;

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
        public async Task StillSubscribedToUserAfterOneOfMultipleConnectionsAssociatedWithUserDisconnects()
        {
            using (var client1 = new TestClient())
            using (var client2 = new TestClient())
            using (var client3 = new TestClient())
            {
                var manager = BackplaneHarness.HubLifetimeManager;

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

namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using System;
    using NHibernate;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class SubscriptionRepositoryTests
    {
        private MockRepository mocks;
        private SubscriptionRepository repo;
        private ISessionFactory mockSessionFactory;
        private ISession sess;
        private ICriteria crit;
        private ITransaction mockTransaction;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockSessionFactory = mocks.CreateMock<ISessionFactory>();
            repo = new SubscriptionRepository(mockSessionFactory);

            sess = mocks.CreateMock<ISession>();
            crit = mocks.CreateMock<ICriteria>();
            mockTransaction = mocks.CreateMock<ITransaction>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            repo = null;
            mockSessionFactory = null;

            sess = null;
            crit = null;
            mockTransaction = null;
        }

        [Test]
        public void When_Adding_A_Message_That_Doesnt_Exist()
        {
            StoredSubscription subs = new StoredSubscription("a", "m");

            using(mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(sess.BeginTransaction()).Return(mockTransaction);
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<StoredSubscription>()).Return(null);
                Expect.Call(sess.Save(null)).Return(subs).IgnoreArguments();

                mockTransaction.Commit();
                mockTransaction.Dispose();
                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Add("a", new Uri("msmq://localhost/test_client"));
            }
        }

        [Test]
        public void When_Adding_A_Message_That_Does_Exist()
        {
            StoredSubscription subs = new StoredSubscription("a","m");
            using (mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(sess.BeginTransaction()).Return(mockTransaction);
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<StoredSubscription>()).Return(subs);
                sess.Update(subs);

                mockTransaction.Commit();
                mockTransaction.Dispose();
                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Add("a", new Uri("msmq://localhost/test_client"));
            }
        }

        [Test]
        public void When_Deactivating_A_Message_That_Doesnt_Exist()
        {
            StoredSubscription subs = new StoredSubscription("a", "m");

            using (mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(sess.BeginTransaction()).Return(mockTransaction);
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<StoredSubscription>()).Return(null);

                mockTransaction.Commit();
                mockTransaction.Dispose();
                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Remove("a", new Uri("msmq://localhost/test_client"));
            }
        }

        [Test]
        public void When_Deactivating_A_Message_That_Does_Exist()
        {
            StoredSubscription subs = new StoredSubscription("a", "m");
            using (mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(sess.BeginTransaction()).Return(mockTransaction);
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<StoredSubscription>()).Return(subs);
                sess.Update(subs);

                mockTransaction.Commit();
                mockTransaction.Dispose();
                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Remove("a", new Uri("msmq://localhost/test_client"));
            }
        }
    }
}
namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using NHibernate;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class SubscriptionRepositoryTests
    {
        private MockRepository mocks;
        private SubscriptionRepository repo;
        private ISessionFactory mockSessionFactory;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockSessionFactory = mocks.CreateMock<ISessionFactory>();
            repo = new SubscriptionRepository(mockSessionFactory);
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            repo = null;
            mockSessionFactory = null;
        }

        [Test]
        public void When_Adding_A_Message_That_Doesnt_Exist()
        {
            ISession sess = mocks.CreateMock<ISession>();
            ICriteria crit = mocks.CreateMock<ICriteria>();
            Subscription subs = new Subscription("a", "m");

            using(mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<Subscription>()).Return(null);
                Expect.Call(sess.Save(null)).Return(subs).IgnoreArguments();

                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Add(new Subscription("a", "m"));
            }
        }

        [Test]
        public void When_Adding_A_Message_That_Does_Exist()
        {
            ISession sess = mocks.CreateMock<ISession>();
            ICriteria crit = mocks.CreateMock<ICriteria>();
            Subscription subs = new Subscription("a","m");
            using (mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<Subscription>()).Return(subs);
                sess.Update(subs);

                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Add(subs);
            }
        }

        [Test]
        public void When_Deactivating_A_Message_That_Doesnt_Exist()
        {
            ISession sess = mocks.CreateMock<ISession>();
            ICriteria crit = mocks.CreateMock<ICriteria>();
            Subscription subs = new Subscription("a", "m");

            using (mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<Subscription>()).Return(null);

                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Deactivate(new Subscription("a", "m"));
            }
        }

        [Test]
        public void When_Deactivating_A_Message_That_Does_Exist()
        {
            ISession sess = mocks.CreateMock<ISession>();
            ICriteria crit = mocks.CreateMock<ICriteria>();
            Subscription subs = new Subscription("a", "m");
            using (mocks.Record())
            {
                Expect.Call(mockSessionFactory.OpenSession()).Return(sess);
                Expect.Call(sess.CreateCriteria(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.Add(null)).Return(crit).IgnoreArguments();
                Expect.Call(crit.UniqueResult<Subscription>()).Return(subs);
                sess.Update(subs);

                sess.Dispose();
            }
            using (mocks.Playback())
            {
                repo.Deactivate(subs);
            }
        }
    }
}
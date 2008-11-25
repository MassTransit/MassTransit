// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests
{
    using Castle.Core;
    using Castle.Windsor;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    
    using WindsorIntegration;
    using FollowerRepository=MassTransit.Subscriptions.FollowerRepository;
    using ISubscriptionCache=MassTransit.Subscriptions.ISubscriptionCache;


    [TestFixture]
    public class SubscriptionManagerContext
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _container = new DefaultMassTransitContainer("subscriptions.castle.xml");
            _container.AddComponentLifeStyle("followerRepository", typeof (FollowerRepository), LifestyleType.Singleton);

            _subscriptionBus = _container.Resolve<IServiceBus>("subscriptions");
            _subscriptionService = _container.Resolve<SubscriptionService>();
            _subscriptionService.Start();

            _localBus = _container.Resolve<IServiceBus>("local");
            _remoteBus = _container.Resolve<IServiceBus>("remote");

            _subscriptionCache = _container.Resolve<ISubscriptionCache>("SubscriptionServiceCache");
            Before_each();
        }

        [TearDown]
        public void Teardown()
        {
            After_each();


            _localBus.Dispose();
            _container.Release(_localBus);

            _remoteBus.Dispose();
            _container.Release(_remoteBus);

            _subscriptionService.Stop();
            _subscriptionService.Dispose();
            _container.Release(_subscriptionService);

            _subscriptionBus.Dispose();
            _container.Release(_subscriptionBus);

            _container.Dispose();
        }

        #endregion

        private IWindsorContainer _container;
        private IServiceBus _localBus;
        private IServiceBus _remoteBus;
        private IServiceBus _subscriptionBus;
        private MassTransit.Subscriptions.SubscriptionService _subscriptionService;
        private ISubscriptionCache _subscriptionCache;

        public MassTransit.Subscriptions.SubscriptionService SubscriptionService
        {
            get { return _subscriptionService; }
        }

        protected virtual void Before_each()
        {
        }

        protected virtual void After_each()
        {
        }

        public IServiceBus LocalBus
        {
            get { return _localBus; }
        }

        public IServiceBus RemoteBus
        {
            get { return _remoteBus; }
        }

        public IServiceBus SubscriptionBus
        {
            get { return _subscriptionBus; }
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public ISubscriptionCache SubscriptionCache
        {
            get { return _subscriptionCache; }
        }
    }
}
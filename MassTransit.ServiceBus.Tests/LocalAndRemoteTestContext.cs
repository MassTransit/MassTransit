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
namespace MassTransit.ServiceBus.Tests
{
    using System;
    using Castle.Windsor;
    using NUnit.Framework;
    using Rhino.Mocks;
    using WindsorIntegration;

    [TestFixture]
    public class LocalAndRemoteTestContext
    {
        [SetUp]
        public void Setup()
        {
            _mocks = new MockRepository();

            _container = new DefaultMassTransitContainer("loopback.castle.xml");

            _localBus = _container.Resolve<IServiceBus>("local");
            _remoteBus = _container.Resolve<IServiceBus>("remote");

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

            _container.Dispose();

            _mocks = null;
        }

        private MockRepository _mocks;

        private IWindsorContainer _container;
        private IServiceBus _localBus;
        private IServiceBus _remoteBus;

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

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        protected T Mock<T>()
        {
            return _mocks.DynamicMock<T>();
        }

        protected T StrictMock<T>()
        {
            return _mocks.CreateMock<T>();
        }

        protected T Stub<T>()
        {
            return _mocks.Stub<T>();
        }

        protected void ReplayAll()
        {
            _mocks.ReplayAll();
        }

        protected void Verify(object o)
        {
            _mocks.Verify(o);
        }

        protected void VerifyAll()
        {
            _mocks.VerifyAll();
        }

        protected IDisposable Record
        {
            get { return _mocks.Record(); }
        }

        protected IDisposable Playback
        {
            get { return _mocks.Playback(); }
        }
    }
}
// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading;
    using Magnum.Extensions;


    public class SimpleConsumer :
        Consumes<SimpleMessageInterface>.All
    {
        static readonly ManualResetEvent _consumerCreated = new ManualResetEvent(false);
        static SimpleConsumer _lastConsumer;
        readonly ISimpleConsumerDependency _dependency;
        readonly ManualResetEvent _received;
        SimpleMessageInterface _lastMessage;

        public SimpleConsumer(ISimpleConsumerDependency dependency)
        {
            _dependency = dependency;
            Console.WriteLine("SimpleConsumer()");

            _received = new ManualResetEvent(false);

            _lastConsumer = this;
            _consumerCreated.Set();
        }

        public SimpleMessageInterface Last
        {
            get
            {
                if (_received.WaitOne(8.Seconds()))
                    return _lastMessage;

                throw new TimeoutException("Timeout waiting for message to be consumed");
            }
        }

        public ISimpleConsumerDependency Dependency
        {
            get { return _dependency; }
        }

        public static SimpleConsumer LastConsumer
        {
            get
            {
                if (_consumerCreated.WaitOne(8.Seconds()))
                    return _lastConsumer;

                throw new TimeoutException("Timeout waiting for consumer to be created");
            }
        }

        public void Consume(SimpleMessageInterface message)
        {
            _dependency.DoSomething();


            _lastMessage = message;
            _received.Set();
        }
    }
}
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


    public class SimpleConsumerDependency :
        ISimpleConsumerDependency
    {
        bool _disposed;

        public void Dispose()
        {
            _disposed = true;
        }

        public bool WasDisposed
        {
            get { return _disposed; }
        }

        public void DoSomething()
        {
            if (_disposed)
                throw new ObjectDisposedException("Should not have disposed of me just yet");

            SomethingDone = true;
        }

        public bool SomethingDone { get; private set; }
    }
}
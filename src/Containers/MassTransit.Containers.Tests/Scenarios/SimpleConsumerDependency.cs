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
namespace MassTransit.Containers.Tests.Scenarios
{
    using System;
    using System.Threading.Tasks;
    using Internals.Extensions;


    public class SimpleConsumerDependency :
        ISimpleConsumerDependency,
        IDisposable
    {
        readonly TaskCompletionSource<bool> _disposed;

        public SimpleConsumerDependency()
        {
            _disposed = TaskCompletionSourceFactory.New<bool>();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing Simple Dependency");

            _disposed.TrySetResult(true);
        }

        public Task<bool> WasDisposed
        {
            get { return _disposed.Task; }
        }

        public void DoSomething()
        {
            if (_disposed.Task.IsCompleted)
                throw new ObjectDisposedException("Should not have disposed of me just yet");

            SomethingDone = true;
        }

        public bool SomethingDone { get; private set; }
    }
}

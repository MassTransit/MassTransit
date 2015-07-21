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
namespace MassTransit.Util
{
    using System.Threading;
    using System.Threading.Tasks;


    public class StopSignal
    {
        readonly TaskCompletionSource<bool> _stopTask;
        readonly CancellationTokenSource _stopToken;
        CancellationTokenRegistration _registration;

        public StopSignal()
        {
            _stopTask = new TaskCompletionSource<bool>();

            _stopToken = new CancellationTokenSource();
            _registration = _stopToken.Token.Register(() =>
            {
                _stopTask.TrySetResult(true);

                _registration.Dispose();
            });
        }

        public Task Stopped => _stopTask.Task;
        public CancellationToken CancellationToken => _stopToken.Token;

        public void Stop()
        {
            _stopToken.Cancel();
        }
    }
}
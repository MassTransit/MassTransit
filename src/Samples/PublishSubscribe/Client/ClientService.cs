// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Client
{
    using System;
    using MassTransit;
    using SecurityMessages;

    public class ClientService
    {
        IServiceBus _bus;

        public void Start(IServiceBus bus)
        {
            _bus = bus;
            _bus.SubscribeConsumer<PasswordUpdater>();

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));

            var message = new RequestPasswordUpdate(newPassword);

            _bus.Publish(message);

            Console.WriteLine("Waiting For Reply with CorrelationId '{0}'", message.CorrelationId);
            Console.WriteLine(new string('-', 20));
        }

        public void Stop()
        {
            //do nothing
        }
    }
}
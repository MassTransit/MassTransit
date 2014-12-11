// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using RabbitMqTransport;


    public class RabbitMqConnection_Specs
	{
		RabbitMqConnection _conn = new RabbitMqConnection(
			TestFactory.ConnectionFactory());

		[When]
		public void Disposing_Managed()
		{
			_conn.Dispose();
		}

		[Then]
		public void Second_Dispose_Throws_ObjectDisposedException()
		{
			Assert.Throws<ObjectDisposedException>(
				() => _conn.Dispose());
		}
	}
}
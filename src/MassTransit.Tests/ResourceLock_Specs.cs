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
	using System;
	using Magnum.DateTimeExtensions;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Threading;

	[TestFixture]
	public class ResourceLock_Specs
	{
		[Test]
		public void The_resource_lock_should_prevent_multiple_entry_from_occuring()
		{
			IEndpoint endpoint = MockRepository.GenerateMock<IEndpoint>();

			ResourceLock<IEndpoint> resourceLock = new ResourceLock<IEndpoint>(endpoint, 1);

			using (var firstLock = resourceLock.AcquireLock(1.Seconds()))
			{
				try
				{
					using (var secondLock = resourceLock.AcquireLock(100.Milliseconds()))
					{
						Assert.Fail("We should not have gotten a second lock");
					}

					Assert.Fail("We should have thrown an exception on the second lock");
				}
				catch (Exception ex)
				{
					Assert.IsInstanceOfType(typeof(ResourceLockException), ex);
				}
			}
		}

		[Test]
		public void The_resource_lock_should_allow_up_to_the_limit_and_no_more()
		{
			IEndpoint endpoint = MockRepository.GenerateMock<IEndpoint>();

			ResourceLock<IEndpoint> resourceLock = new ResourceLock<IEndpoint>(endpoint, 2);

			using (var firstLock = resourceLock.AcquireLock(100.Milliseconds()))
			{
				using (var secondLock = resourceLock.AcquireLock(100.Milliseconds()))
				{
					try
					{
						using (var thirdLock = resourceLock.AcquireLock(100.Milliseconds()))
						{
							Assert.Fail("We should not have gotten a third lock");
						}

						Assert.Fail("We should have thrown an exception on the third lock");

					}
					catch (Exception ex)
					{
						Assert.IsInstanceOfType(typeof (ResourceLockException), ex);
					}
				}
			}
		}

		[Test]
		public void A_lot_of_folks_using_the_lock_should_be_fine()
		{
			IEndpoint endpoint = MockRepository.GenerateMock<IEndpoint>();

			ResourceLock<IEndpoint> resourceLock = new ResourceLock<IEndpoint>(endpoint, 1);

			using (var firstLock = resourceLock.AcquireLock(100.Milliseconds()))
			{
			}

			using (var firstLock = resourceLock.AcquireLock(100.Milliseconds()))
			{
			}

			using (var firstLock = resourceLock.AcquireLock(100.Milliseconds()))
			{
			}

			using (var firstLock = resourceLock.AcquireLock(100.Milliseconds()))
			{
			}

			using (var firstLock = resourceLock.AcquireLock(100.Milliseconds()))
			{
			}
		}
	}
}
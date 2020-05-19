// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Groups
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;


    [TestFixture]
	public class Group_Specs
	{
		private Guid _transactionId;

        [SetUp]
		protected  void SetUp()
		{
			_transactionId = Guid.NewGuid();
		}


		[Test]
		public void It_should_be_easy_to_build_and_send_a_group_of_messages()
		{
			var createOrder = new CreateOrder(_transactionId);

			var orderItemList = new List<AddOrderItem>();

			var messageGroup = createOrder.CombineWith(orderItemList.ToArray());

			Assert.AreEqual(1, messageGroup.Count());
		}

		[Test]
		public void It_should_be_easy_to_build_and_send_a_group_of_messages_with_multiple_messages()
		{
			var createOrder = new CreateOrder(_transactionId);

			var orderItemList = new List<AddOrderItem> { new AddOrderItem(_transactionId), new AddOrderItem(_transactionId) };

			var messageGroup = createOrder.CombineWith(orderItemList.ToArray());

			Assert.AreEqual(3, messageGroup.Count());
		}

		[Test]
		public void Multiple_groups_should_be_combinable_into_a_single_group()
		{
			var createOrder = new CreateOrder(_transactionId);

			var orderItemList = new List<AddOrderItem> { new AddOrderItem(_transactionId), new AddOrderItem(_transactionId) };

			var messageGroup = createOrder.CombineWith(orderItemList.ToArray());

			var secondGroup = new AddOrderItem(_transactionId).CombineWith(new AddOrderItem(_transactionId));

			messageGroup.CombineWith(secondGroup);

			Assert.AreEqual(5, messageGroup.Count());
			
		}
	}

	public static class OIUWERO
	{
		public static CorrelatedMessageGroup<TKey> CombineWith<TMessage, TKey>(this CorrelatedBy<TKey> message, params TMessage[] messages)
			where TMessage : CorrelatedBy<TKey>
		{
			var group = new CorrelatedMessageGroup<TKey> {message};

			group.AddRange(messages);

			return group;
		}
	}

	[Serializable]
	public class CorrelatedMessageGroup<TKey> :
		IMessageGroup
	{
		private readonly List<object> _messages = new List<object>();


		public void Add<TMessage>(TMessage message) 
			where TMessage : CorrelatedBy<TKey>
		{
			_messages.Add(message);
		}

		public void AddRange<TMessage>(IEnumerable<TMessage> messages)
			where TMessage : CorrelatedBy<TKey>
		{
			foreach (var message in messages)
			{
				Add(message);
			}
		}

		public CorrelatedMessageGroup<TKey> CombineWith<TMessage>(params TMessage[] messages)
			where TMessage : CorrelatedBy<TKey>
		{
			AddRange(messages);

			return this;
		}

		public CorrelatedMessageGroup<TKey> CombineWith(params CorrelatedMessageGroup<TKey>[] otherGroups)
		{
			for (int i = 0; i < otherGroups.Length; i++)
			{
				_messages.AddRange(otherGroups[i]);
			}

			return this;
		}

		public IEnumerator<object> GetEnumerator()
		{
			return _messages.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public interface IMessageGroup :
		IEnumerable<object>
	{
	}

	[Serializable]
	public class AddOrderItem :
		CorrelatedBy<Guid>
	{
		private readonly Guid _transactionId;

		public AddOrderItem(Guid transactionId)
		{
			_transactionId = transactionId;
		}

		public Guid CorrelationId
		{
			get { return _transactionId; }
		}
	}

	[Serializable]
	public class CreateOrder : 
		CorrelatedBy<Guid>
	{
		private readonly Guid _transactionId;

		public CreateOrder(Guid transactionId)
		{
			_transactionId = transactionId;
		}

		public Guid CorrelationId
		{
			get { return _transactionId; }
		}
	}
}
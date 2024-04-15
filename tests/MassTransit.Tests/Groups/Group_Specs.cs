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
        [Test]
        public void It_should_be_easy_to_build_and_send_a_group_of_messages()
        {
            var createOrder = new CreateOrder(_transactionId);

            var orderItemList = new List<AddOrderItem>();

            CorrelatedMessageGroup<Guid> messageGroup = createOrder.CombineWith(orderItemList.ToArray());

            Assert.That(messageGroup.Count(), Is.EqualTo(1));
        }

        [Test]
        public void It_should_be_easy_to_build_and_send_a_group_of_messages_with_multiple_messages()
        {
            var createOrder = new CreateOrder(_transactionId);

            var orderItemList = new List<AddOrderItem>
            {
                new AddOrderItem(_transactionId),
                new AddOrderItem(_transactionId)
            };

            CorrelatedMessageGroup<Guid> messageGroup = createOrder.CombineWith(orderItemList.ToArray());

            Assert.That(messageGroup.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Multiple_groups_should_be_combinable_into_a_single_group()
        {
            var createOrder = new CreateOrder(_transactionId);

            var orderItemList = new List<AddOrderItem>
            {
                new AddOrderItem(_transactionId),
                new AddOrderItem(_transactionId)
            };

            CorrelatedMessageGroup<Guid> messageGroup = createOrder.CombineWith(orderItemList.ToArray());

            CorrelatedMessageGroup<Guid> secondGroup = new AddOrderItem(_transactionId).CombineWith(new AddOrderItem(_transactionId));

            messageGroup.CombineWith(secondGroup);

            Assert.That(messageGroup.Count(), Is.EqualTo(5));
        }

        [SetUp]
        protected void SetUp()
        {
            _transactionId = Guid.NewGuid();
        }

        Guid _transactionId;
    }


    public static class OIUWERO
    {
        public static CorrelatedMessageGroup<TKey> CombineWith<TMessage, TKey>(this CorrelatedBy<TKey> message, params TMessage[] messages)
            where TMessage : CorrelatedBy<TKey>
        {
            var group = new CorrelatedMessageGroup<TKey> { message };

            group.AddRange(messages);

            return group;
        }
    }


    [Serializable]
    public class CorrelatedMessageGroup<TKey> :
        IMessageGroup
    {
        readonly List<object> _messages = new List<object>();

        public IEnumerator<object> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add<TMessage>(TMessage message)
            where TMessage : CorrelatedBy<TKey>
        {
            _messages.Add(message);
        }

        public void AddRange<TMessage>(IEnumerable<TMessage> messages)
            where TMessage : CorrelatedBy<TKey>
        {
            foreach (var message in messages)
                Add(message);
        }

        public CorrelatedMessageGroup<TKey> CombineWith<TMessage>(params TMessage[] messages)
            where TMessage : CorrelatedBy<TKey>
        {
            AddRange(messages);

            return this;
        }

        public CorrelatedMessageGroup<TKey> CombineWith(params CorrelatedMessageGroup<TKey>[] otherGroups)
        {
            for (var i = 0; i < otherGroups.Length; i++)
                _messages.AddRange(otherGroups[i]);

            return this;
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
        public AddOrderItem(Guid transactionId)
        {
            CorrelationId = transactionId;
        }

        public Guid CorrelationId { get; }
    }


    [Serializable]
    public class CreateOrder :
        CorrelatedBy<Guid>
    {
        public CreateOrder(Guid transactionId)
        {
            CorrelationId = transactionId;
        }

        public Guid CorrelationId { get; }
    }
}

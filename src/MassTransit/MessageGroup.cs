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
namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using Magnum;
	using Magnum.Reflection;

	/// <summary>
	/// A message group allows a set of messages to be sent as a single message to a single consumer with the intent
	/// that the messages will be processed as a single unit of work.
	/// </summary>
	[Serializable]
	public class MessageGroup
	{
		/// <summary>
		/// Creates a message group with the specified list of messages. This class is normally built by the <c>MessageGroupBuilder</c>
		/// </summary>
		/// <param name="messages">The messages included in the group</param>
		public MessageGroup(List<object> messages)
		{
			Messages = messages;
		}

		protected MessageGroup()
		{
		}

		public List<object> Messages { get; set; }

		/// <summary>
		/// The number of messages in the message group.
		/// </summary>
		public int Count
		{
			get { return Messages.Count; }
		}

		/// <summary>
		/// Returns the message from the group at the specified index
		/// </summary>
		/// <param name="index">The index of the message to return</param>
		/// <returns>The message at the specified index</returns>
		public object this[int index]
		{
			get { return Messages[index]; }
		}

		/// <summary>
		/// Returns a builder for the message group of type <typeparamref name="TGroup"/>
		/// </summary>
		/// <typeparam name="TGroup">The type of message group to be built</typeparam>
		/// <returns>A message builder for <typeparamref name="TGroup"/></returns>
		public static MessageGroupBuilder<TGroup> Build<TGroup>() where TGroup : class
		{
			return new MessageGroupBuilder<TGroup>();
		}

		/// <summary>
		/// Builds a list of the messages specified and returns a message group for the messages
		/// </summary>
		/// <param name="items">The messages to store in the message group</param>
		/// <returns>A standard message group</returns>
		public static MessageGroupBuilder<TGroup> Join<TGroup>(params object[] items) where TGroup : class
		{
			List<object> messages = new List<object>(items);

			return new MessageGroupBuilder<TGroup>(messages);
		}

		public static MessageGroup Join(params object[] items)
		{
			return Join<MessageGroup>(items);
		}

		public object[] ToArray()
		{
			return Messages.ToArray();
		}

		public T Get<T>(int index)
		{
            Guard.LessThan(Messages.Count, index, "index");

			Type typeofT = typeof (T);

			object obj = Messages[index];

			Type objType = obj.GetType();

			if (typeofT.IsAssignableFrom(objType))
				return (T) obj;

			throw new ArgumentException(string.Format("The message at the specified index could not be converted to the type specified ({0} -> {1})", objType, typeofT));
		}

		public void Split(IServiceBus bus)
		{
			foreach (object message in Messages)
			{
				RepublishMessage(message, bus);
			}
		}

		private static void RepublishMessage(object message, IServiceBus bus)
		{
			bus.FastInvoke(x => x.Publish(message), message);
		}
	}

	/// <summary>
	/// Prepares the contents of a message group in order to create the class of the specified type.
	/// </summary>
	/// <typeparam name="TBuilder">The type of class to create for the message group</typeparam>
	public class MessageGroupBuilder<TBuilder> where TBuilder : class
	{
		internal static readonly AllowMessageTypeAttribute _allow;
		internal readonly List<object> _messages = new List<object>();

		static MessageGroupBuilder()
		{
			object[] attributes = typeof (TBuilder).GetCustomAttributes(typeof (AllowMessageTypeAttribute), false);
			if (attributes != null && attributes.Length > 0)
			{
				_allow = attributes[0] as AllowMessageTypeAttribute;
			}
		}

		public MessageGroupBuilder(List<object> messages)
		{
			messages.ForEach(message => Add(message));
		}

		public MessageGroupBuilder()
		{
		}

		/// <summary>
		/// Adds a message to the message group
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public MessageGroupBuilder<TBuilder> Add(object message)
		{
			Type typeofT = message.GetType();

			if (MessageTypeAllowed(typeofT))
				_messages.Add(message);
			else
			{
				throw new ArgumentException(typeofT.FullName + " is not an allowed message type for this group", "message");
			}

			return this;
		}

		private static bool MessageTypeAllowed(Type t)
		{
			if (_allow == null)
				return true;

			return _allow.GetUsage(t) != MessageTypeUsage.None;
		}

		/// <summary>
		/// Converts the <c>MessageGroupBuilder</c> into a new instance of type <c>TBuilder</c>
		/// </summary>
		/// <param name="builder">The builder to convert</param>
		/// <returns>A new instance of the message group class</returns>
		public static implicit operator TBuilder(MessageGroupBuilder<TBuilder> builder)
		{
			return FastActivator<TBuilder>.Create(builder._messages);
			//return Activator.CreateInstance(typeof (TBuilder), builder._messages) as TBuilder;
		}
	}
}
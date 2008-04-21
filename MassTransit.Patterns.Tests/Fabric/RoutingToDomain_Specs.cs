namespace MassTransit.Patterns.Tests.Fabric
{
	using System;
	using System.Text;
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using ServiceBus;

	[TestFixture]
	public class When_a_message_destination_includes_a_domain_class
	{
		public class LoginController :
			Patterns.Fabric.IConsume<LoginUserSuccess>,
			IProduce<LoginUser>
		{
			private Patterns.Fabric.IConsume<LoginUser> _consumer;

			public void Consume(LoginUserSuccess message)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				DetachConsumer(null);
			}

			public void AttachConsumer(Patterns.Fabric.IConsume<LoginUser> consumer)
			{
				_consumer = consumer;
			}

			public void DetachConsumer(Patterns.Fabric.IConsume<LoginUser> consumer)
			{
				_consumer = null;
			}

			public void LoginUser(string username, string password)
			{
				LoginUser loginUser = new LoginUser(username, password);

				_consumer.Consume(loginUser);
			}
		}

		public class Session :
			IProduce<LoginUserSuccess>,
			Patterns.Fabric.IConsume<LoginUser>
		{
			public void Consume(LoginUser message)
			{
				throw new NotImplementedException();
			}

			public void AttachConsumer(Patterns.Fabric.IConsume<LoginUserSuccess> consumer)
			{
				throw new NotImplementedException();
			}

			public void DetachConsumer(Patterns.Fabric.IConsume<LoginUserSuccess> consumer)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}

		public interface IDomainMessage : IMessage
		{
			Type RootType { get; }
		}

		public interface IDomainMessage<TRoot, TKey> : IDomainMessage
		{
			TKey To { get; }
		}

		public class LoginUser : IDomainMessage<Session, Guid>
		{
			private readonly string _password;
			private readonly string _username;
			private readonly Guid _to;

			public LoginUser(string username, string password)
			{
				_username = username;
				_password = password;
				_to = Guid.Empty;
			}

			public Guid To
			{
				get { return _to; }
			}

			public Type RootType
			{
				get { return typeof (Session); }
			}

			public string Username
			{
				get { return _username; }
			}

			public string Password
			{
				get { return _password; }
			}
		}

		public class LoginUserSuccess : IMessage
		{
		}

		[Test]
		public void An_object_should_be_resurrected_and_invoked()
		{
			IServiceMesh mesh = new ServiceMesh();



			LoginController controller = new LoginController();



			mesh.AddComponent(controller);


			//controller.LoginUser("username", "password");
		}
	}

	public interface IServiceMesh
	{
		void AddComponent(object component);
	}

	public class ServiceMesh : IServiceMesh
	{
		public void AddComponent(object component)
		{
			StringBuilder componentInfo = new StringBuilder();

			Type componentType = component.GetType();

			componentInfo.Append("Component ");
			componentInfo.Append(componentType.Name);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if(interfaceType.IsGenericType)
				{
					Type[] genericArgs = interfaceType.GetGenericArguments();
					if(genericArgs.Length == 1)
					{
						if(typeof(IMessage).IsAssignableFrom(genericArgs[0]))
						{
							Type produceType = typeof (IProduce<>).MakeGenericType(genericArgs[0]);
							if(produceType.IsAssignableFrom(componentType))
							{
								componentInfo.AppendFormat(", Produces({0})", genericArgs[0].Name);
							}

							Type consumeType = typeof (Patterns.Fabric.IConsume<>).MakeGenericType(genericArgs[0]);
							if(consumeType.IsAssignableFrom(componentType))
							{
								componentInfo.AppendFormat(", Consumes({0})", genericArgs[0].Name);
							}
						}
					}
				}
			}

			System.Diagnostics.Debug.WriteLine(componentInfo.ToString());
		}
	}
}
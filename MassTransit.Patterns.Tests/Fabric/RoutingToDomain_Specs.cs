namespace MassTransit.Patterns.Tests.Fabric
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;
	using MassTransit.Patterns.Fabric;
	using NUnit.Framework;
	using ServiceBus;
	using IConsume = MassTransit.Patterns.Fabric;

	[TestFixture]
	public class When_a_message_destination_includes_a_domain_class
	{
		public class LoginController :
			Patterns.Fabric.IConsume<LoginUserSuccess>,
			IProduce<LoginUser>
		{
			private readonly IServiceMesh _mesh;
			private Patterns.Fabric.IConsume<LoginUser> _consumer;

			public LoginController(IServiceMesh mesh)
			{
				_mesh = mesh;
			}

			#region IConsume<LoginUserSuccess> Members

			public void Consume(LoginUserSuccess message)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				Detach(null);
			}

			#endregion

			#region IProduce<LoginUser> Members

			public void Attach(Patterns.Fabric.IConsume<LoginUser> consumer)
			{
				_consumer = consumer;
			}

			public void Detach(Patterns.Fabric.IConsume<LoginUser> consumer)
			{
				_consumer = null;
			}

			#endregion

			public void LoginUser(string username, string password)
			{
				LoginUser loginUser = new LoginUser(username, password);

				_mesh.Produce(this, loginUser);
			}
		}

		public class Session :
			IProduce<LoginUserSuccess>,
			Patterns.Fabric.IConsume<LoginUser>
		{
			#region IConsume<LoginUser> Members

			public void Consume(LoginUser message)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IProduce<LoginUserSuccess> Members

			public void Attach(Patterns.Fabric.IConsume<LoginUserSuccess> consumer)
			{
				throw new NotImplementedException();
			}

			public void Detach(Patterns.Fabric.IConsume<LoginUserSuccess> consumer)
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			#endregion
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
			private readonly Guid _to;
			private readonly string _username;

			public LoginUser(string username, string password)
			{
				_username = username;
				_password = password;
				_to = Guid.Empty;
			}

			public string Username
			{
				get { return _username; }
			}

			public string Password
			{
				get { return _password; }
			}

			#region IDomainMessage<Session,Guid> Members

			public Guid To
			{
				get { return _to; }
			}

			public Type RootType
			{
				get { return typeof (Session); }
			}

			#endregion
		}

		public class LoginUserSuccess : IMessage
		{
		}

		public class SimpleTransformer : ITransformer<LoginUser, LoginUserSuccess>
		{
			private readonly IServiceMesh _mesh;

			public SimpleTransformer(IServiceMesh mesh)
			{
				_mesh = mesh;
			}

			#region ITransformer<LoginUser,LoginUserSuccess> Members

			public void Consume(LoginUser message)
			{
				_mesh.Produce(this, new LoginUserSuccess());
			}

			public void Dispose()
			{
			}

			public void Attach(Patterns.Fabric.IConsume<LoginUserSuccess> consumer)
			{
			}

			public void Detach(Patterns.Fabric.IConsume<LoginUserSuccess> consumer)
			{
			}

			#endregion
		}


		[Test]
		public void An_object_should_be_resurrected_and_invoked()
		{
			IServiceMesh mesh = new ServiceMesh();


			mesh.AddComponent(typeof (LoginController));

			mesh.AddComponent(typeof (SimpleTransformer));

			mesh.Route<LoginController>().To<SimpleTransformer>().End();
			mesh.Route<SimpleTransformer>().To<LoginController>().End();

			LoginController controller = mesh.Resolve<LoginController>();

			controller.LoginUser("username", "password");
		}
	}


	public interface IServiceMesh
	{
		void AddComponent(object component);
		void AddComponent(Type componentType);
		TComponent Resolve<TComponent>();

		void Produce<TComponent, TMessage>(TComponent source, TMessage message) where TComponent : IProduce<TMessage> where TMessage : IMessage;
		IComponentRoute<TSource> Route<TSource>();
		void AddRoute(IComponentRoute route);
	}

	public class ServiceMesh : IServiceMesh
	{
		private readonly List<Type> _componentTypes = new List<Type>();
		private readonly Dictionary<Type, List<Type>> _route = new Dictionary<Type, List<Type>>();

		#region IServiceMesh Members

		public void AddComponent(object component)
		{
			Type componentType = component.GetType();

			AddComponent(componentType);
		}

		public void AddComponent(Type componentType)
		{
			if (!_componentTypes.Contains(componentType))
				_componentTypes.Add(componentType);

			StringBuilder componentInfo = new StringBuilder();

			componentInfo.Append("Component ");
			componentInfo.Append(componentType.Name);

			foreach (Type interfaceType in componentType.GetInterfaces())
			{
				if (interfaceType.IsGenericType)
				{
					Type[] genericArgs = interfaceType.GetGenericArguments();
					if (genericArgs.Length == 2)
					{
						if (typeof (IMessage).IsAssignableFrom(genericArgs[0]) && typeof (IMessage).IsAssignableFrom(genericArgs[1]))
						{
							Type transformerType = typeof (ITransformer<,>).MakeGenericType(genericArgs[0], genericArgs[1]);
							if (transformerType.IsAssignableFrom(componentType))
							{
								componentInfo.AppendFormat(", Transforms({0} => {1})", genericArgs[0].Name, genericArgs[1].Name);
							}
						}
					}
					else if (genericArgs.Length == 1)
					{
						if (typeof (IMessage).IsAssignableFrom(genericArgs[0]))
						{
							Type produceType = typeof (IProduce<>).MakeGenericType(genericArgs[0]);
							if (produceType.IsAssignableFrom(componentType))
							{
								componentInfo.AppendFormat(", Produces({0})", genericArgs[0].Name);
							}

							Type consumeType = typeof (Patterns.Fabric.IConsume<>).MakeGenericType(genericArgs[0]);
							if (consumeType.IsAssignableFrom(componentType))
							{
								componentInfo.AppendFormat(", Consumes({0})", genericArgs[0].Name);
							}
						}
					}
				}
			}

			Debug.WriteLine(componentInfo.ToString());
		}

		public TComponent Resolve<TComponent>()
		{
			if (!_componentTypes.Contains(typeof (TComponent)))
				throw new ArgumentException("The type was not found in the container", typeof (TComponent).FullName);

			object[] args = new object[] {this};

			TComponent component = (TComponent) Activator.CreateInstance(typeof (TComponent), args);

			return component;
		}

		public void Produce<TComponent, TMessage>(TComponent source, TMessage message)
			where TComponent : IProduce<TMessage>
			where TMessage : IMessage
		{
			StringBuilder trace = new StringBuilder();

			trace.AppendFormat("Routing {0} for {1}", typeof (TMessage).Name, typeof (TComponent).Name);

			if (_route.ContainsKey(typeof (TComponent)))
			{
				foreach (Type t in _route[typeof (TComponent)])
				{
					Type consumerType = typeof (Patterns.Fabric.IConsume<>).MakeGenericType(typeof (TMessage));
					if (consumerType.IsAssignableFrom(t))
					{
						trace.AppendFormat(" to {0}", t.Name);
					}
				}
			}

			Debug.WriteLine(trace.ToString());
		}

		public IComponentRoute<TComponent> Route<TComponent>()
		{
			return new ComponentRoute<TComponent>(this);
		}

		public void AddRoute(IComponentRoute route)
		{
			if (!_componentTypes.Contains(route.SourceType))
				AddComponent(route.SourceType);

			if (!_componentTypes.Contains(route.TargetType))
				AddComponent(route.TargetType);

			foreach (Type interfaceType in route.SourceType.GetInterfaces())
			{
				if (interfaceType.IsGenericType)
				{
					Type[] genericArgs = interfaceType.GetGenericArguments();
					if (genericArgs.Length == 1)
					{
						if (typeof (IMessage).IsAssignableFrom(genericArgs[0]))
						{
							Type produceType = typeof (IProduce<>).MakeGenericType(genericArgs[0]);
							if (produceType.IsAssignableFrom(route.SourceType))
							{
								Type consumerType = typeof (Patterns.Fabric.IConsume<>).MakeGenericType(genericArgs[0]);
								if (consumerType.IsAssignableFrom(route.TargetType))
								{
									if (!_route.ContainsKey(route.SourceType))
										_route.Add(route.SourceType, new List<Type>());

									_route[route.SourceType].Add(route.TargetType);
								}
							}
						}
					}
				}
			}
		}

		#endregion
	}

	public class ComponentRoute<TSource> : IComponentRoute<TSource>
	{
		private readonly IServiceMesh _mesh;
		private readonly Type _sourceType;
		private Type _targetType;

		public ComponentRoute(IServiceMesh mesh)
		{
			_mesh = mesh;
			_sourceType = typeof (TSource);
		}

		#region IComponentRoute<TSource> Members

		public Type TargetType
		{
			get { return _targetType; }
		}

		public Type SourceType
		{
			get { return _sourceType; }
		}

		public IComponentRoute<TSource> To<TTarget>()
		{
			_targetType = typeof (TTarget);

			return this;
		}

		public void End()
		{
			_mesh.AddRoute(this);
		}

		#endregion
	}

	public interface IComponentRoute
	{
		Type TargetType { get; }

		Type SourceType { get; }
	}

	public interface IComponentRoute<TSource> : IComponentRoute
	{
		IComponentRoute<TSource> To<TTarget>();
		void End();
	}
}
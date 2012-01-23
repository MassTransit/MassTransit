namespace MassTransit.SubscriptionConnectors
{
	using System.Collections.Generic;
	using System.Linq;
	using Util;

	/// <summary>
	/// Helper class for providing the message reflection for consumers.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal static class ConsumptionReflector<T>
		where T : class
	{
		internal static IEnumerable<ConsumptionPair> AllDistinctMessages()
		{
			return ConsumesSelectedContextMessages()
				.Concat(ConsumesContextMessages())
				.Concat(ConsumesSelectedMessages())
				.Concat(ConsumesAllMessages())
				.Distinct((x, y) => x.MessageType == y.MessageType);
		}

		internal static IEnumerable<ConsumptionPair> ConsumesContextMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsGenericType)
				.Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
				.Select(x => new ConsumptionPair(x.InterfaceType, x.MessageType.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false);
		}

		internal static IEnumerable<ConsumptionPair> ConsumesSelectedContextMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.Selected))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsGenericType)
				.Where(x => x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>))
				.Select(x => new ConsumptionPair(x.InterfaceType, x.MessageType.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false);
		}

		internal static IEnumerable<ConsumptionPair> ConsumesAllMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.All))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x =>
				       !(x.MessageType.IsGenericType &&
				         x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)));
		}

		internal static IEnumerable<ConsumptionPair> ConsumesSelectedMessages()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType)
				.Where(x => x.GetGenericTypeDefinition() == typeof(Consumes<>.Selected))
				.Select(x => new ConsumptionPair(x, x.GetGenericArguments()[0]))
				.Where(x => x.MessageType.IsValueType == false)
				.Where(x =>
				       !(x.MessageType.IsGenericType &&
				         x.MessageType.GetGenericTypeDefinition() == typeof(IConsumeContext<>)));
		}
	}
}
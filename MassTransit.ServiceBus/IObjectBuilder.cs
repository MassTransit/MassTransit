namespace MassTransit.ServiceBus
{
	using System;

	public interface IObjectBuilder
	{
		/// <summary>
		/// Build an object of the specified type
		/// </summary>
		/// <param name="objectType">The type of object to build</param>
		/// <returns>The object that was built</returns>
		object Build(Type objectType);

		/// <summary>
		/// Build an object of the specified type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <returns></returns>
		T Build<T>(Type type) where T : class;

		/// <summary>
		/// Releases an object back to the container
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		void Release<T>(T obj);
	}
}
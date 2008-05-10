namespace CodeCamp.Core
{
	public interface IRepository<T> where T : IIdentifier
	{
		T Get(object id);
	}

	public interface IIdentifier
	{
		object Key { get; }
	}
}
namespace Automatonymous
{
	public delegate bool ConditionFactory<in TInstance, in TData>(BehaviorContext<TInstance, TData> context)
		where TData : class;

}
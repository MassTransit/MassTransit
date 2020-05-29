namespace MassTransit.Testing.Indicators
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public abstract class BaseBusActivityIndicatorConnectable : Connectable<IConditionObserver>,
        IObservableCondition
    {
        public ConnectHandle ConnectConditionObserver(IConditionObserver observer)
        {
            return Connect(observer);
        }

        public abstract bool IsMet { get; }

        protected Task ConditionUpdated()
        {
            return ForEachAsync(x => x.ConditionUpdated());
        }
    }
}

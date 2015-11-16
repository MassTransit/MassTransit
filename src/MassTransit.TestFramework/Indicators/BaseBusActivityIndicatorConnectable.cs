namespace MassTransit.TestFramework.Indicators
{
    using System.Threading.Tasks;
    using Util;


    public abstract class BaseBusActivityIndicatorConnectable : Connectable<IConditionObserver>,
        IObservableCondition
    {
        public ConnectHandle ConnectConditionObserver(IConditionObserver observer)
        {
            return Connect(observer);
        }

        protected Task ConditionUpdated()
        {
            return ForEachAsync(x => x.ConditionUpdated());
        }

        public abstract bool State { get; }
    }
}
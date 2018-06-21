namespace MassTransit.LamarIntegration
{
    using System;


    public sealed class LamarActivityFactory
    {
        static readonly Lazy<LamarActivityFactory> _lazy = new Lazy<LamarActivityFactory>(() => new LamarActivityFactory());

        public static LamarActivityFactory Instance => _lazy.Value;

        public TActivity Get<TActivity, TArguments>(TArguments arguments)
            where TActivity : class
            where TArguments : class
        {
            return Activator.CreateInstance(typeof(TActivity), arguments) as TActivity;
        }
    }
}
namespace MassTransit.Configuration
{
    using System;
    using System.Linq.Expressions;
    using Internals;
    using Saga;


    /// <summary>
    /// Creates a saga instance using the constructor, via a compiled expression. This class
    /// is built asynchronously and hot-wrapped to replace the basic Activator style.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class ConstructorSagaInstanceFactory<TSaga>
        where TSaga : class, ISaga
    {
        public ConstructorSagaInstanceFactory()
        {
            var constructorInfo = typeof(TSaga).GetConstructor(new[] { typeof(Guid) });
            if (constructorInfo == null)
            {
                throw new ArgumentException("The saga does not have a public constructor with a single Guid correlationId parameter: "
                    + TypeCache<TSaga>.ShortName);
            }

            var correlationId = Expression.Parameter(typeof(Guid), "correlationId");
            var @new = Expression.New(constructorInfo, correlationId);

            FactoryMethod = Expression.Lambda<SagaInstanceFactoryMethod<TSaga>>(@new, correlationId).CompileFast();
        }

        public SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
    }
}

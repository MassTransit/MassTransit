namespace MassTransit.Saga.Factories
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using Internals.Reflection;
    using Metadata;
    using Util;


    /// <summary>
    /// Creates a saga instance using the constructor, via a compiled expression. This class
    /// is built asynchronously and hot-wrapped to replace the basic Activator style.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class PropertySagaInstanceFactory<TSaga>
        where TSaga : class, ISaga
    {
        public PropertySagaInstanceFactory()
        {
            var constructorInfo = typeof(TSaga).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new ArgumentException($"The saga {TypeMetadataCache<TSaga>.ShortName} does not have a default public constructor");

            if (!TypeCache<TSaga>.ReadWritePropertyCache.TryGetValue("CorrelationId", out ReadWriteProperty<TSaga> property))
                throw new ArgumentException($"The saga {TypeMetadataCache<TSaga>.ShortName} does not have a writable CorrelationId property");

            ParameterExpression correlationId = Expression.Parameter(typeof(Guid), "correlationId");

            NewExpression newSaga = Expression.New(constructorInfo);

            var saga = Expression.Variable(typeof(TSaga), "saga");

            var assign = Expression.Assign(saga, newSaga);

            MethodCallExpression call = Expression.Call(saga, property.Property.SetMethod, correlationId);

            LabelTarget returnTarget = Expression.Label(typeof(TSaga));

            GotoExpression returnExpression = Expression.Return(returnTarget,
                saga, typeof(TSaga));

            LabelExpression returnLabel = Expression.Label(returnTarget, Expression.Default(typeof(TSaga)));

            var block = Expression.Block(new[] {saga},
                assign,
                call,
                returnExpression,
                returnLabel);

            FactoryMethod = Expression.Lambda<SagaInstanceFactoryMethod<TSaga>>(block, correlationId).CompileFast();
        }

        public SagaInstanceFactoryMethod<TSaga> FactoryMethod { get; }
    }
}

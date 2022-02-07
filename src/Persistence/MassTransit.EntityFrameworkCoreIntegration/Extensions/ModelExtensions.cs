namespace MassTransit.EntityFrameworkCoreIntegration.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata;


    /// <summary>
    /// <see cref="IModel"/> extensions
    /// </summary>
    public static class ModelExtensions
    {
        static readonly Lazy<Func<IModel, Type, IEntityType>> _findEntityType = new Lazy<Func<IModel, Type, IEntityType>>(CreateFindEntityTypeMethod);

        /// <summary>
        ///     Gets the entity that maps the given entity class. Returns <c>null</c> if no entity type with
        ///     the given CLR type is found or the entity type has a defining navigation.
        /// </summary>
        /// <param name="model"> The model to find the entity type in. </param>
        /// <param name="type"> The type to find the corresponding entity type for. </param>
        /// <returns> The entity type, or <c>null</c> if none if found. </returns>
        public static IEntityType SafeFindEntityType(this IModel model, Type type) => _findEntityType.Value(model, type);

        // Bugfix for Github issue #2980 and #3151 - "method not found"
        static Func<IModel, Type, IEntityType> CreateFindEntityTypeMethod()
        {
            var ef6FindEntityType = typeof(IModel)
                .GetMethods()
                .SingleOrDefault(x =>
                    x.Name == "FindEntityType" &&
                    x.ReturnType == typeof(IEntityType) &&
                    x.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(Type) }));
            if (ef6FindEntityType != null)
            {
                return (Func<IModel, Type, IEntityType>)Delegate.CreateDelegate(
                    type: typeof(Func<IModel, Type, IEntityType>), // the delegate has on more argument than the instance method
                    firstArgument: null, // the first argument is the (hidden) instance argument of the instance method
                    method: ef6FindEntityType);
            }

            // EF3 / EF5 fallback
            return Microsoft.EntityFrameworkCore.ModelExtensions.FindEntityType;
        }
    }
}

namespace MassTransit.Persistence.Integration.SqlBuilders
{
    using System.Data;
    using System.Reflection;


    public class ReflectionsAdapter
    {
        /// <summary>
        /// Creates a reflections-based data adapter for the specified <typeparamref name="TModel" />.
        /// </summary>
        /// <typeparam name="TModel">The target type to return</typeparam>
        /// <returns>An instance of a model adapter</returns>
        public static Func<IDataReader, TModel> CreateFor<TModel>()
            where TModel : class
        {
            return new Adapter<TModel>().Convert;
        }
        
        class Adapter<TModel>
        {
            IDictionary<string, int>? _columns;

            public TModel Convert(IDataReader input)
            {
                _columns ??= CreateSchema(input);

                var target = Activator.CreateInstance<TModel>();
                var properties = typeof(TModel).GetProperties().Where(p => p.CanWrite);

                foreach (var property in properties)
                {
                    if (!_columns.TryGetValue(property.Name, out var index))
                        continue;

                    var value = input.GetValue(index);
                    var actual = CheckDbNull(value);

                    if (property.PropertyType == typeof(Guid) && actual is byte[] { Length: 16 } bytes)
                        property.SetValue(target, new Guid(bytes)); // thanks, MySql ...
                    else
                        property.SetValue(target, actual);
                }

                return target;

                object? CheckDbNull(object? value)
                {
                    return DBNull.Value.Equals(value) ? null : value;
                }
            }

            static IDictionary<string, int> CreateSchema(IDataReader input)
            {
                var mappings = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                for (var fieldIndex = 0; fieldIndex < input.FieldCount; fieldIndex++)
                {
                    var fieldName = input.GetName(fieldIndex);

                    mappings[fieldName] = fieldIndex;
                }

                return mappings;
            }
        }
    }
}

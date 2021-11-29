namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Initializers;
    using MassTransit.Initializers.Contexts;
    using MassTransit.Initializers.PropertyProviders;
    using MassTransit.Initializers.Variables;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class The_property_providers
    {
        [Test]
        public async Task Should_support_async_arrays()
        {
            var input = new {IntArray = new[] {Task.FromResult(1), Task.FromResult(2), Task.FromResult(3)}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (long[])default);

            long[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new long[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_async_arrays_with_nulls()
        {
            var input = new {IntArray = new[] {Task.FromResult(1), Task.FromResult(2), null, Task.FromResult(3)}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (long[])default);

            long[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new long[] {1, 2, 0, 3}));
        }

        [Test]
        public async Task Should_support_async_convertible_arrays()
        {
            var input = new {IntArray = Task.FromResult(new[] {1, 2, 3})};

            var provider = GetPropertyProvider(input, x => x.IntArray, (long[])default);

            long[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new long[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_async_input_types()
        {
            var input = new {IntValue = Task.FromResult(27)};

            var provider = GetPropertyProvider(input, x => x.IntValue, 69);

            var intValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(intValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_async_matching_arrays()
        {
            var input = new {IntArray = Task.FromResult(new[] {1, 2, 3})};

            var provider = GetPropertyProvider(input, x => x.IntArray, (int[])default);

            int[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_async_matching_enumerable()
        {
            var input = new {IntArray = Task.FromResult(new[] {1, 2, 3})};

            var provider = GetPropertyProvider(input, x => x.IntArray, (IEnumerable<int>)default);

            IEnumerable<int> listValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(listValue, Is.EqualTo(new List<int>
            {
                1,
                2,
                3
            }));
        }

        [Test]
        public async Task Should_support_async_matching_lists()
        {
            var input = new {IntArray = Task.FromResult(new[] {1, 2, 3})};

            var provider = GetPropertyProvider(input, x => x.IntArray, (List<int>)default);

            List<int> listValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(listValue, Is.EqualTo(new List<int>
            {
                1,
                2,
                3
            }));
        }

        [Test]
        public async Task Should_support_async_matching_readonly_lists()
        {
            var input = new {IntArray = Task.FromResult(new[] {1, 2, 3})};

            var provider = GetPropertyProvider(input, x => x.IntArray, (IReadOnlyList<int>)default);

            IReadOnlyList<int> listValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(listValue, Is.EqualTo(new List<int>
            {
                1,
                2,
                3
            }));
        }

        [Test]
        public async Task Should_support_async_result_convertible_arrays()
        {
            var input = new {IntArray = new[] {1, 2, 3}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (Task<long[]>)default);

            Task<long[]> arrayTask = await provider.GetProperty(CreateInitializeContext(input));

            long[] arrayValue = await arrayTask;

            Assert.That(arrayValue, Is.EqualTo(new long[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_async_result_types()
        {
            var input = new {IntValue = 27};

            var provider = GetPropertyProvider(input, x => x.IntValue, Task.FromResult(69));

            Task<int> valueTask = await provider.GetProperty(CreateInitializeContext(input));
            var intValue = await valueTask;

            Assert.That(intValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_convertible_arrays()
        {
            var input = new {IntArray = new[] {1, 2, 3}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (long[])default);

            long[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new long[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_convertible_arrays_from_strings()
        {
            var input = new {StringArray = new[] {"1", "2", "", "3"}};

            var provider = GetPropertyProvider(input, x => x.StringArray, (int?[])default);

            int?[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new int?[] {1, 2, default, 3}));
        }

        [Test]
        public async Task Should_support_convertible_arrays_to_nullable_types()
        {
            var input = new {IntArray = new[] {1, 2, 3}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (long?[])default);

            long?[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new long?[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_convertible_arrays_to_strings()
        {
            var input = new {IntArray = new[] {1, 2, 3}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (string[])default);

            string[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new[] {"1", "2", "3"}));
        }

        [Test]
        public async Task Should_support_convertible_arrays_with_nullable_types()
        {
            var input = new {IntArray = new int?[] {1, 2, 3}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (long[])default);

            long[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new long[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_convertible_property_types()
        {
            var input = new {IntValue = 27};

            var longProvider = GetPropertyProvider(input, x => x.IntValue, 69L);

            var longValue = await longProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(longValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_dictionary()
        {
            var input = new
            {
                Strings = new Dictionary<string, string>
                {
                    {"Hello", "World"},
                    {"Thank You", "Next"}
                }
            };

            var provider = GetPropertyProvider(input, x => x.Strings, (IDictionary<string, string>)default);

            IDictionary<string, string> dictionary = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(dictionary, Is.Not.Null);
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary["Hello"], Is.EqualTo("World"));
            Assert.That(dictionary["Thank You"], Is.EqualTo("Next"));
        }

        [Test]
        public async Task Should_support_dictionary_key_conversion()
        {
            var input = new
            {
                Strings = new Dictionary<string, string>
                {
                    {"1", "One"},
                    {"2", "Two"}
                }
            };

            var provider = GetPropertyProvider(input, x => x.Strings, (IDictionary<int, string>)default);

            IDictionary<int, string> dictionary = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(dictionary, Is.Not.Null);
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary[1], Is.EqualTo("One"));
            Assert.That(dictionary[2], Is.EqualTo("Two"));
        }

        [Test]
        public async Task Should_support_dictionary_type_conversion()
        {
            var input = new
            {
                Strings = new Dictionary<string, string>
                {
                    {"Hello", "World"},
                    {"Thank You", "Next"}
                }
            };

            var provider = GetPropertyProvider(input, x => x.Strings, (IDictionary<string, object>)default);

            IDictionary<string, object> dictionary = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(dictionary, Is.Not.Null);
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary["Hello"], Is.EqualTo("World"));
            Assert.That(dictionary["Thank You"], Is.EqualTo("Next"));
        }

        [Test]
        public async Task Should_support_enumerable_key_value_pair()
        {
            var input = new
            {
                Strings = (IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string>
                {
                    {"Hello", "World"},
                    {"Thank You", "Next"}
                }
            };

            var provider = GetPropertyProvider(input, x => x.Strings, (IDictionary<string, string>)default);

            IDictionary<string, string> dictionary = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(dictionary, Is.Not.Null);
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary["Hello"], Is.EqualTo("World"));
            Assert.That(dictionary["Thank You"], Is.EqualTo("Next"));
        }

        [Test]
        public async Task Should_support_enums()
        {
            var input = new {Status = TaskStatus.RanToCompletion};

            var provider = GetPropertyProvider(input, x => x.Status, TaskStatus.Canceled);

            var idValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(idValue, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public async Task Should_support_enums_from_int()
        {
            var input = new {Status = (int)TaskStatus.RanToCompletion};

            var provider = GetPropertyProvider(input, x => x.Status, TaskStatus.Canceled);

            var idValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(idValue, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public async Task Should_support_enums_from_long()
        {
            var input = new {Status = (long)TaskStatus.RanToCompletion};

            var provider = GetPropertyProvider(input, x => x.Status, TaskStatus.Canceled);

            var idValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(idValue, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public async Task Should_support_enums_from_strings()
        {
            var input = new {Status = TaskStatus.RanToCompletion.ToString()};

            var provider = GetPropertyProvider(input, x => x.Status, TaskStatus.Canceled);

            var idValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(idValue, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public async Task Should_support_exceptions()
        {
            var input = new {Exception = new IntentionalTestException("It Happens")};

            var provider = GetPropertyProvider(input, x => x.Exception, (ExceptionInfo)default);

            var exceptionInfo = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(exceptionInfo, Is.Not.Null);
            Assert.That(exceptionInfo.Message, Is.EqualTo("It Happens"));
        }

        [Test]
        public async Task Should_support_initializer_variables()
        {
            var id = NewId.NextGuid();

            var input = new {IdValue = new IdVariable(id)};

            var provider = GetPropertyProvider(input, x => x.IdValue, Guid.Empty);

            var idValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(idValue, Is.EqualTo(id));
        }

        [Test]
        public async Task Should_support_initializer_variables_to_strings()
        {
            var id = NewId.NextGuid();

            var input = new {IdValue = new IdVariable(id)};

            var provider = GetPropertyProvider(input, x => x.IdValue, (string)default);

            var idValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(idValue, Is.EqualTo(id.ToString()));
        }

        [Test]
        public async Task Should_support_interface_initializer()
        {
            var input = new {Message = new {Text = "Hello"}};

            var provider = GetPropertyProvider(input, x => x.Message, (MessageContract)default);

            var message = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(message, Is.Not.Null);
            Assert.That(message.Text, Is.EqualTo("Hello"));
        }

        [Test]
        public async Task Should_support_interface_initializer_nested()
        {
            var input = new
            {
                Message = new
                {
                    Text = "Hello",
                    Headers = new object[]
                    {
                        new
                        {
                            Key = "Format",
                            Value = "CSV"
                        },
                        new
                        {
                            Key = "Length",
                            Value = 2457
                        }
                    }
                }
            };

            var provider = GetPropertyProvider(input, x => x.Message, (MessageContract)default);

            var message = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(message, Is.Not.Null);
            Assert.That(message.Text, Is.EqualTo("Hello"));
            Assert.That(message.Headers, Is.Not.Null);
            Assert.That(message.Headers.Length, Is.EqualTo(2));
            Assert.That(message.Headers[0].Key, Is.EqualTo("Format"));
            Assert.That(message.Headers[0].Value, Is.EqualTo("CSV"));
            Assert.That(message.Headers[1].Key, Is.EqualTo("Length"));
            Assert.That(message.Headers[1].Value, Is.EqualTo("2457"));
        }

        [Test]
        public async Task Should_support_interface_initializer_with_array()
        {
            var input = new {Messages = new[] {new {Text = "Hello"}, new {Text = "World"}}};

            var provider = GetPropertyProvider(input, x => x.Messages, (MessageContract[])default);

            MessageContract[] message = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(message, Is.Not.Null);
            Assert.That(message.Length, Is.EqualTo(2));
            Assert.That(message[0].Text, Is.EqualTo("Hello"));
            Assert.That(message[1].Text, Is.EqualTo("World"));
        }

        [Test]
        public async Task Should_support_interface_initializer_with_dictionary()
        {
            var input = new
            {
                Messages = new Dictionary<string, object>
                {
                    {"Hello", new {Text = "Hello"}},
                    {"World", new {Text = "World"}}
                }
            };

            var provider = GetPropertyProvider(input, x => x.Messages, (IDictionary<string, MessageContract>)default);

            IDictionary<string, MessageContract> message = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(message, Is.Not.Null);
            Assert.That(message.Count, Is.EqualTo(2));
            Assert.That(message["Hello"].Text, Is.EqualTo("Hello"));
            Assert.That(message["World"].Text, Is.EqualTo("World"));
        }

        [Test]
        public async Task Should_support_matching_arrays()
        {
            var input = new {IntArray = new[] {1, 2, 3}};

            var provider = GetPropertyProvider(input, x => x.IntArray, (int[])default);

            int[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new[] {1, 2, 3}));
        }

        [Test]
        public async Task Should_support_matching_enumerable()
        {
            var input = new {Decimals = Enumerable.Repeat(98.7m, 2)};

            var provider = GetPropertyProvider(input, x => x.Decimals, (decimal[])default);

            decimal[] arrayValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(arrayValue, Is.EqualTo(new[] {98.7m, 98.7m}));
        }

        [Test]
        public async Task Should_support_matching_property_types()
        {
            var input = new {IntValue = 27};

            var provider = GetPropertyProvider(input, x => x.IntValue);

            var intValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(intValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_multiple_result_types_for_single_property()
        {
            var input = new {IntValue = 27};

            var provider = GetPropertyProvider(input, x => x.IntValue);

            var intValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(intValue, Is.EqualTo(27));

            var longProvider = GetPropertyProvider(input, x => x.IntValue, 69L);

            var longValue = await longProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(longValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_nullable_input_types()
        {
            var input = new {IntValue = (int?)27};

            var provider = GetPropertyProvider(input, x => x.IntValue, 69);

            var intValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(intValue, Is.EqualTo(27));

            var longProvider = GetPropertyProvider(input, x => x.IntValue, 69L);

            var longValue = await longProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(longValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_nullable_result_types()
        {
            var input = new {IntValue = 27};

            var provider = GetPropertyProvider(input, x => x.IntValue, (int?)69);

            int? intValue = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(intValue, Is.EqualTo(27));

            var longProvider = GetPropertyProvider(input, x => x.IntValue, (long?)69L);

            long? longValue = await longProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(longValue, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_object()
        {
            var input = new {IntValue = 27};

            var provider = GetPropertyProvider(input, x => x.IntValue, (object)default);

            var value = await provider.GetProperty(CreateInitializeContext(input));

            Assert.That(value, Is.EqualTo(27));
        }

        [Test]
        public async Task Should_support_string_to_uri()
        {
            var input = new {Address = "http://localhost/"};

            var uriProvider = GetPropertyProvider(input, x => x.Address, (Uri)default);

            var uriValue = await uriProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(uriValue, Is.EqualTo(new Uri(input.Address)));
        }

        [Test]
        public async Task Should_support_uri()
        {
            var input = new {Address = new Uri("http://localhost/")};

            var uriProvider = GetPropertyProvider(input, x => x.Address, (Uri)default);

            var uriValue = await uriProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(uriValue, Is.EqualTo(input.Address));
        }

        [Test]
        public async Task Should_support_uri_to_string()
        {
            var input = new {Address = new Uri("http://localhost/")};

            var stringProvider = GetPropertyProvider(input, x => x.Address, (string)default);

            var stringValue = await stringProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(stringValue, Is.EqualTo("http://localhost/"));
        }

        [Test]
        public async Task Should_support_value_to_string_types()
        {
            var input = new {IntValue = 27};

            var stringProvider = GetPropertyProvider(input, x => x.IntValue, (string)default);

            var stringValue = await stringProvider.GetProperty(CreateInitializeContext(input));

            Assert.That(stringValue, Is.EqualTo("27"));
        }


        class Subject
        {
        }


        public interface MessageContract
        {
            string Text { get; }

            MessageHeader[] Headers { get; }
        }


        public interface MessageHeader
        {
            string Key { get; }
            string Value { get; }
        }


        InitializeContext<Subject, TInput> CreateInitializeContext<TInput>(TInput input)
            where TInput : class
        {
            var baseContext = new BaseInitializeContext(CancellationToken.None);
            InitializeContext<Subject> messageContext = baseContext.CreateMessageContext(new Subject());

            return messageContext.CreateInputContext(input);
        }

        IPropertyProvider<TInput, TProperty> GetPropertyProvider<TInput, TProperty>(TInput input, Expression<Func<TInput, TProperty>> propertyExpression)
            where TInput : class
        {
            var propertyInfo = propertyExpression.GetPropertyInfo();

            var propertyProviderFactory = new PropertyProviderFactory<TInput>();
            if (propertyProviderFactory.TryGetPropertyProvider<TProperty>(propertyInfo, out IPropertyProvider<TInput, TProperty> provider))
                return provider;

            throw new InvalidOperationException("Unable to create provider");
        }

        IPropertyProvider<TInput, TResult> GetPropertyProvider<TInput, TProperty, TResult>(TInput input, Expression<Func<TInput, TProperty>>
            propertyExpression, TResult defaultValue)
            where TInput : class
        {
            var propertyInfo = propertyExpression.GetPropertyInfo();

            var propertyProviderFactory = new PropertyProviderFactory<TInput>();
            if (propertyProviderFactory.TryGetPropertyProvider<TResult>(propertyInfo, out IPropertyProvider<TInput, TResult> provider))
                return provider;

            throw new InvalidOperationException("Unable to create provider");
        }
    }
}

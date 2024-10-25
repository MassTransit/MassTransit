namespace MassTransit.AzureTable.Saga
{
    using System;
    using Initializers;
    using Initializers.TypeConverters;
    using Internals;


    public class EntityPropertyTypeConverter :
        ITypeConverter<object, bool>,
        ITypeConverter<object, bool?>,
        ITypeConverter<bool, object>,
        ITypeConverter<bool?, object>,
        ITypeConverter<object, int>,
        ITypeConverter<object, int?>,
        ITypeConverter<int, object>,
        ITypeConverter<int?, object>,
        ITypeConverter<object, long>,
        ITypeConverter<object, long?>,
        ITypeConverter<long, object>,
        ITypeConverter<long?, object>,
        ITypeConverter<object, double>,
        ITypeConverter<object, double?>,
        ITypeConverter<double, object>,
        ITypeConverter<double?, object>,
        ITypeConverter<object, Guid>,
        ITypeConverter<object, Guid?>,
        ITypeConverter<Guid, object>,
        ITypeConverter<Guid?, object>,
        ITypeConverter<object, DateTime>,
        ITypeConverter<object, DateTime?>,
        ITypeConverter<DateTime, object>,
        ITypeConverter<DateTime?, object>,
        ITypeConverter<object, TimeSpan>,
        ITypeConverter<object, TimeSpan?>,
        ITypeConverter<TimeSpan, object>,
        ITypeConverter<TimeSpan?, object>,
        ITypeConverter<object, DateTimeOffset>,
        ITypeConverter<object, DateTimeOffset?>,
        ITypeConverter<DateTimeOffset, object>,
        ITypeConverter<DateTimeOffset?, object>,
        ITypeConverter<object, byte[]>,
        ITypeConverter<byte[], object>,
        ITypeConverter<object, Uri>,
        ITypeConverter<Uri, object>,
        ITypeConverter<object, Version>,
        ITypeConverter<Version, object>,
        ITypeConverter<object, string>,
        ITypeConverter<string, object>
    {
        public static readonly EntityPropertyTypeConverter Instance = new EntityPropertyTypeConverter();
        static readonly TimeSpanTypeConverter _timeSpanConverter = new TimeSpanTypeConverter();

        EntityPropertyTypeConverter()
        {
        }

        public bool TryConvert(object input, out bool result)
        {
            if (input is bool value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out bool? result)
        {
            if (input is bool value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out byte[] result)
        {
            if (input is byte[] value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out DateTime result)
        {
            if (input is DateTime dt)
            {
                result = dt;
                return true;
            }

            if(input is DateTimeOffset dto)
            {
                result = dto.UtcDateTime;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out DateTime? result)
        {
            if (input is DateTime dt)
            {
                result = dt;
                return true;
            }

            if (input is DateTimeOffset dto)
            {
                result = dto.UtcDateTime;
                return true;
            }


            result = default;
            return false;
        }

        public bool TryConvert(object input, out DateTimeOffset result)
        {
            if (input is DateTimeOffset dto)
            {
                result = dto;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out DateTimeOffset? result)
        {
            if (input is DateTimeOffset dto)
            {
                result = dto;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out double result)
        {
            if (input is double)
            {
                result = input as double? ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out double? result)
        {
            if (input is double value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out Guid result)
        {
            if (input is Guid)
            {
                result = input as Guid? ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out Guid? result)
        {
            if (input is Guid value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out int result)
        {
            if (input is int)
            {
                result = input as int? ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out int? result)
        {
            if (input is int value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out long result)
        {
            if (input is long)
            {
                result = input as long? ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out long? result)
        {
            if (input is long value)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(bool? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(bool input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(byte[] input, out object result)
        {
            if (input != null)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTime? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTime input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(DateTimeOffset? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTimeOffset input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(double? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(double input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(Guid? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Guid input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(int? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(int input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(long? input, out object result)
        {
            if (input.HasValue)
            {
                result = input;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(long input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(string input, out object result)
        {
            result = input;
            return true;
        }

        public bool TryConvert(TimeSpan? input, out object result)
        {
            if (input.HasValue && _timeSpanConverter.TryConvert(input.Value, out var text))
            {
                result = text;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(TimeSpan input, out object result)
        {
            if (_timeSpanConverter.TryConvert(input, out var text))
            {
                result = text;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Uri input, out object result)
        {
            if (input != null)
            {
                result = input.ToString();
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Version input, out object result)
        {
            if (input != null)
            {
                result = input.ToString();
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out string result)
        {
            if (input is string value
                && input.ToString() != null)
            {
                result = value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out TimeSpan result)
        {
            if (input is string)
            {
                result = string.IsNullOrWhiteSpace(input.ToString())
                    ? default
                    : _timeSpanConverter.TryConvert(input.ToString(), out var value)
                        ? value
                        : default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out TimeSpan? result)
        {
            if (input is string)
            {
                result = string.IsNullOrWhiteSpace(input.ToString())
                    ? default
                    : _timeSpanConverter.TryConvert(input.ToString(), out var value)
                        ? value
                        : default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out Uri result)
        {
            if (input is string
                && input.ToString() != null
                && Uri.IsWellFormedUriString(input.ToString(), UriKind.RelativeOrAbsolute))
            {
                result = new Uri(input.ToString());
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(object input, out Version result)
        {
            if (input is string
                && input.ToString() != null
                && Version.TryParse(input.ToString(), out var version))
            {
                result = version;
                return true;
            }

            result = default;
            return false;
        }

        public static bool IsSupported(Type propertyType)
        {
            var fromType = typeof(ITypeConverter<,>).MakeGenericType(typeof(object), propertyType);
            var toType = typeof(ITypeConverter<,>).MakeGenericType(propertyType, typeof(object));

            return typeof(EntityPropertyTypeConverter).HasInterface(fromType) && typeof(EntityPropertyTypeConverter).HasInterface(toType);
        }
    }
}

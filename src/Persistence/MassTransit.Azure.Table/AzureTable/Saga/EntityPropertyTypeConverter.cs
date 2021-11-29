namespace MassTransit.AzureTable.Saga
{
    using System;
    using Initializers;
    using Microsoft.Azure.Cosmos.Table;


    public class EntityPropertyTypeConverter :
        ITypeConverter<EntityProperty, bool>,
        ITypeConverter<EntityProperty, bool?>,
        ITypeConverter<bool, EntityProperty>,
        ITypeConverter<bool?, EntityProperty>,
        ITypeConverter<EntityProperty, int>,
        ITypeConverter<EntityProperty, int?>,
        ITypeConverter<int, EntityProperty>,
        ITypeConverter<int?, EntityProperty>,
        ITypeConverter<EntityProperty, long>,
        ITypeConverter<EntityProperty, long?>,
        ITypeConverter<long, EntityProperty>,
        ITypeConverter<long?, EntityProperty>,
        ITypeConverter<EntityProperty, double>,
        ITypeConverter<EntityProperty, double?>,
        ITypeConverter<double, EntityProperty>,
        ITypeConverter<double?, EntityProperty>,
        ITypeConverter<EntityProperty, Guid>,
        ITypeConverter<EntityProperty, Guid?>,
        ITypeConverter<Guid, EntityProperty>,
        ITypeConverter<Guid?, EntityProperty>,
        ITypeConverter<EntityProperty, DateTime>,
        ITypeConverter<EntityProperty, DateTime?>,
        ITypeConverter<DateTime, EntityProperty>,
        ITypeConverter<DateTime?, EntityProperty>,
        ITypeConverter<EntityProperty, DateTimeOffset>,
        ITypeConverter<EntityProperty, DateTimeOffset?>,
        ITypeConverter<DateTimeOffset, EntityProperty>,
        ITypeConverter<DateTimeOffset?, EntityProperty>,
        ITypeConverter<EntityProperty, byte[]>,
        ITypeConverter<byte[], EntityProperty>,
        ITypeConverter<EntityProperty, Uri>,
        ITypeConverter<Uri, EntityProperty>,
        ITypeConverter<EntityProperty, Version>,
        ITypeConverter<Version, EntityProperty>,
        ITypeConverter<EntityProperty, string>,
        ITypeConverter<string, EntityProperty>
    {
        public static readonly EntityPropertyTypeConverter Instance = new EntityPropertyTypeConverter();

        EntityPropertyTypeConverter()
        {
        }

        public bool TryConvert(bool input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(bool? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out bool result)
        {
            if (input.PropertyType == EdmType.Boolean)
            {
                result = input.BooleanValue.HasValue && input.BooleanValue.Value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out bool? result)
        {
            if (input.PropertyType == EdmType.Boolean)
            {
                result = input.BooleanValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(int input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(int? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out int result)
        {
            if (input.PropertyType == EdmType.Int32)
            {
                result = input.Int32Value ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out int? result)
        {
            if (input.PropertyType == EdmType.Int32)
            {
                result = input.Int32Value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(long input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(long? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out long result)
        {
            if (input.PropertyType == EdmType.Int64)
            {
                result = input.Int64Value ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out long? result)
        {
            if (input.PropertyType == EdmType.Int64)
            {
                result = input.Int64Value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(double input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(double? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out double result)
        {
            if (input.PropertyType == EdmType.Double)
            {
                result = input.DoubleValue ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out double? result)
        {
            if (input.PropertyType == EdmType.Double)
            {
                result = input.DoubleValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Guid input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(Guid? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out Guid result)
        {
            if (input.PropertyType == EdmType.Guid)
            {
                result = input.GuidValue ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out Guid? result)
        {
            if (input.PropertyType == EdmType.Guid)
            {
                result = input.GuidValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTime input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(DateTime? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out DateTime result)
        {
            if (input.PropertyType == EdmType.DateTime)
            {
                result = input.DateTime ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out DateTime? result)
        {
            if (input.PropertyType == EdmType.DateTime)
            {
                result = input.DateTime;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(DateTimeOffset input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(DateTimeOffset? input, out EntityProperty result)
        {
            if (input.HasValue)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out DateTimeOffset result)
        {
            if (input.PropertyType == EdmType.DateTime)
            {
                result = input.DateTimeOffsetValue ?? default;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out DateTimeOffset? result)
        {
            if (input.PropertyType == EdmType.DateTime)
            {
                result = input.DateTimeOffsetValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(byte[] input, out EntityProperty result)
        {
            if (input != null)
            {
                result = new EntityProperty(input);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out byte[] result)
        {
            if (input.PropertyType == EdmType.Binary)
            {
                result = input.BinaryValue;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Uri input, out EntityProperty result)
        {
            if (input != null)
            {
                result = new EntityProperty(input.ToString());
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out Uri result)
        {
            if (input.PropertyType == EdmType.String
                && input.StringValue != null
                && Uri.IsWellFormedUriString(input.StringValue, UriKind.RelativeOrAbsolute))
            {
                result = new Uri(input.StringValue);
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(Version input, out EntityProperty result)
        {
            if (input != null)
            {
                result = new EntityProperty(input.ToString());
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(EntityProperty input, out Version result)
        {
            if (input.PropertyType == EdmType.String
                && input.StringValue != null
                && Version.TryParse(input.StringValue, out var version))
            {
                result = version;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryConvert(string input, out EntityProperty result)
        {
            result = new EntityProperty(input);
            return true;
        }

        public bool TryConvert(EntityProperty input, out string result)
        {
            if (input.PropertyType == EdmType.String
                && input.StringValue != null)
            {
                result = input.StringValue;
                return true;
            }

            result = default;
            return false;
        }
    }
}

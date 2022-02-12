namespace MassTransit
{
    using System;


    public static class ValidationResultExtensions
    {
        public static ValidationResult Failure(this ISpecification configurator, string message)
        {
            return new Result(ValidationResultDisposition.Failure, message);
        }

        public static ValidationResult Failure(this ISpecification configurator, string key, string message)
        {
            return new Result(ValidationResultDisposition.Failure, key, message);
        }

        public static ValidationResult Failure(this ISpecification configurator, string key, string value, string message)
        {
            return new Result(ValidationResultDisposition.Failure, key, value, message);
        }

        public static ValidationResult Warning(this ISpecification configurator, string message)
        {
            return new Result(ValidationResultDisposition.Warning, message);
        }

        public static ValidationResult Warning(this ISpecification configurator, string key, string message)
        {
            return new Result(ValidationResultDisposition.Warning, key, message);
        }

        public static ValidationResult Warning(this ISpecification configurator, string key, string value, string message)
        {
            return new Result(ValidationResultDisposition.Warning, key, value, message);
        }

        public static ValidationResult Success(this ISpecification configurator, string message)
        {
            return new Result(ValidationResultDisposition.Success, message);
        }

        public static ValidationResult Success(this ISpecification configurator, string key, string message)
        {
            return new Result(ValidationResultDisposition.Success, key, message);
        }

        public static ValidationResult Success(this ISpecification configurator, string key, string value, string message)
        {
            return new Result(ValidationResultDisposition.Success, key, value, message);
        }

        public static ValidationResult WithParentKey(this ValidationResult result, string parentKey)
        {
            //string key = result.Key.Contains(".") ? result.Key.Substring(result.Key.IndexOf('.')) : "";

            var key = parentKey + "." + result.Key;

            return new Result(result.Disposition, key, result.Value, result.Message);
        }


        [Serializable]
        public class Result :
            ValidationResult
        {
            public Result(ValidationResultDisposition disposition, string key, string? value, string message)
            {
                Disposition = disposition;
                Key = key;
                Value = value;
                Message = message;
            }

            public Result(ValidationResultDisposition disposition, string key, string message)
            {
                Disposition = disposition;
                Key = key;
                Message = message;
            }

            public Result(ValidationResultDisposition disposition, string message)
            {
                Key = "";
                Disposition = disposition;
                Message = message;
            }

            public ValidationResultDisposition Disposition { get; }
            public string Key { get; }
            public string? Value { get; }
            public string Message { get; }

            public override string ToString()
            {
                return $"[{Disposition}] {(string.IsNullOrEmpty(Key) ? Message : Key + " " + Message)}";
            }
        }
    }
}

namespace MassTransit.DependencyInjection
{
    using Microsoft.Extensions.Options;


    public class ValidateMassTransitHostOptions :
        IValidateOptions<MassTransitHostOptions>
    {
        public ValidateOptionsResult Validate(string name, MassTransitHostOptions options)
        {
            if (options.StopTimeout < options.ConsumerStopTimeout)
                return ValidateOptionsResult.Fail($"{nameof(options.ConsumerStopTimeout)} should be less than or equals to ${nameof(options.StopTimeout)}");

            return ValidateOptionsResult.Success;
        }
    }
}

namespace MassTransit.Definition
{
    /// <summary>
    /// Formats the endpoint names using kebab-case (dashed snake case)
    /// SubmitOrderConsumer -> submit-order
    /// OrderState -> order-state
    /// UpdateCustomerActivity -> update-customer-execute, update-customer-compensate
    /// </summary>
    public class KebabCaseEndpointNameFormatter :
        SnakeCaseEndpointNameFormatter
    {
        protected KebabCaseEndpointNameFormatter()
            : base("-")
        {
        }

        public KebabCaseEndpointNameFormatter(bool includeNamespace)
            : base("-", includeNamespace)
        {
        }

        public new static IEndpointNameFormatter Instance { get; } = new KebabCaseEndpointNameFormatter();

        public override string SanitizeName(string name)
        {
            return base.SanitizeName(name).Replace('_', '-');
        }
    }
}

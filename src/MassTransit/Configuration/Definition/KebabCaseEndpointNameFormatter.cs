namespace MassTransit.Definition
{
    /// <summary>
    /// Formats the endpoint names using kebab-case (dashed snake case)
    ///
    /// SubmitOrderConsumer -> submit-order
    /// OrderState -> order-state
    /// UpdateCustomerActivity -> update-customer-execute, update-customer-compensate
    ///
    /// </summary>
    public class KebabCaseEndpointNameFormatter :
        SnakeCaseEndpointNameFormatter
    {
        public KebabCaseEndpointNameFormatter()
            : base("-")
        {
        }

        public new static IEndpointNameFormatter Instance { get; } = new KebabCaseEndpointNameFormatter();

        protected override string SanitizeName(string name)
        {
            return base.SanitizeName(name).Replace('_', '-');
        }
    }
}

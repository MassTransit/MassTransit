namespace MassTransit.KafkaIntegration.Definition
{
    using Topology;


    /// <summary>
    /// Formats the topic names using kebab-case (dashed snake case)
    /// SubmitOrder-> submit-order
    /// OrderState -> order-state
    /// </summary>
    public class KebabCaseTopicNameFormatter :
        SnakeCaseTopicNameFormatter
    {
        public KebabCaseTopicNameFormatter()
            : base("-")
        {
        }

        public new static IEntityNameFormatter Instance { get; } = new KebabCaseTopicNameFormatter();

        protected override string SanitizeName(string name)
        {
            return base.SanitizeName(name).Replace('_', '-');
        }
    }
}

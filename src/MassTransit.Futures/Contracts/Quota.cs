namespace MassTransit.Contracts
{
    public interface Quota
    {
        /// <summary>
        /// The number of requests requested, if making an allocation for rate purposes
        /// </summary>
        long Quota { get; }

        /// <summary>
        /// The condition under which quota would be allocated
        /// </summary>
        QuotaCondition Condition { get; }
    }
}
namespace MassTransit.MessageData
{
    /// <summary>
    /// Little utility class to help the various implementations.
    /// </summary>
    public static class ClaimChecks
    {
        /// <summary>
        /// Extracts the ID from a claim-check Uri.  Expected format is
        /// `urn:claim?15f9026d4b494b0c93ea80c55224d04c`.
        /// </summary>
        public static Guid Unpack(Uri uri)
        {
            if (uri.Scheme != "urn")
                throw new InvalidOperationException("URI must be a urn");

            if (uri.AbsolutePath != "claim" || string.IsNullOrWhiteSpace(uri.Query) || uri.Query.Length < 2)
                throw new InvalidOperationException("Invalid claim urn format");

            return Guid.Parse(uri.Query.Substring(1));
        }

        /// <summary>
        /// Packs a Guid into a claim-check Uri format.  Returned value
        /// is similar to `urn:claim?15f9026d4b494b0c93ea80c55224d04c`.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Uri Pack(Guid id)
        {
            return new Uri($"urn:claim?{id:N}");
        }

        // whatever ... it's far enough that I'm not going to care, and chances are you won't either.
        static readonly DateTimeOffset MessageDataExpiration = new(2099, 12, 31, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Calculates the future time given now and a nullable offset.  If the offset is
        /// null, then a fixed value of 2099-12-31 is returned.
        /// </summary>
        public static DateTimeOffset GetExpiration(DateTimeOffset now, TimeSpan? timeToLive)
        {
            return timeToLive is null
                ? MessageDataExpiration
                : now.Add(timeToLive.Value);
        }
    }
}

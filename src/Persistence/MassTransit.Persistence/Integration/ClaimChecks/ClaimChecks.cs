namespace MassTransit.MessageData
{
    public static class ClaimChecks
    {
        public static Guid Unpack(Uri uri)
        {
            if (uri.Scheme != "urn")
                throw new InvalidOperationException("URI must be a urn");

            if (uri.AbsolutePath != "claim" || string.IsNullOrWhiteSpace(uri.Query) || uri.Query.Length < 2)
                throw new InvalidOperationException("Invalid claim urn format");

            return Guid.Parse(uri.Query.Substring(1));
        }

        public static Uri Pack(Guid id)
        {
            return new Uri($"urn:claim?{id:N}");
        }

        // whatever ... it's far enough that I'm not going to care, and chances are you won't either.
        static readonly DateTimeOffset MessageDataExpiration = new(2099, 12, 31, 0, 0, 0, TimeSpan.Zero);
        public static DateTimeOffset GetExpiration(DateTimeOffset now, TimeSpan? timeToLive)
        {
            return timeToLive is null
                ? MessageDataExpiration
                : now.Add(timeToLive.Value);
        }
    }
}

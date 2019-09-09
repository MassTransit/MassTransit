namespace MassTransit.AutofacIntegration
{
    /// <summary>
    /// Uses a string-based header to identify the lifetime scope
    /// </summary>
    public class StringHeaderLifetimeScopeIdProvider :
        ILifetimeScopeIdProvider<string>
    {
        readonly ConsumeContext _consumeContext;
        readonly string _headerKey;

        public StringHeaderLifetimeScopeIdProvider(ConsumeContext consumeContext, string headerKey)
        {
            _consumeContext = consumeContext;
            _headerKey = headerKey;
        }

        public bool TryGetScopeId(out string id)
        {
            id = _consumeContext.Headers.Get<string>(_headerKey);

            return !string.IsNullOrWhiteSpace(id);
        }
    }
}

namespace nu.Utility
{
    public class ArgumentMapFactory : IArgumentMapFactory
    {
        #region IArgumentMapFactory Members

        public IArgumentMap CreateMap(object obj)
        {
            return new ArgumentMap(obj.GetType());
        }

        #endregion
    }
}
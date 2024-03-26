namespace MassTransit
{
    using System;


    [Serializable]
    public class InvalidLicenseFormatException :
        Exception
    {
        public InvalidLicenseFormatException()
            : this("The license format was not recognized")
        {
        }

        public InvalidLicenseFormatException(string message)
            : base(message)
        {
        }
    }
}

namespace MassTransit
{
    using System;


    [Serializable]
    public class InvalidLicenseException :
        Exception
    {
        public InvalidLicenseException()
            : this("The license was not valid")
        {
        }

        public InvalidLicenseException(string message)
            : base(message)
        {
        }
    }
}

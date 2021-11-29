namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public class Fry
    {
        public Guid FryId { get; set; }
        public Size Size { get; set; }

        public override string ToString()
        {
            return $"{Size} Fry";
        }
    }
}

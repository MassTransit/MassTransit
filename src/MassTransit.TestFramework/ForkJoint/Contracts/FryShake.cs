namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public class FryShake
    {
        public Guid FryShakeId { get; set; }
        public string Flavor { get; set; }
        public Size Size { get; set; }

        public override string ToString()
        {
            return $"{Size} {Flavor} FryShake";
        }
    }
}

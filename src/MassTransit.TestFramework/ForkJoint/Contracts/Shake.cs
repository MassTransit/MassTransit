namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public class Shake
    {
        public Guid ShakeId { get; set; }
        public string Flavor { get; set; }
        public Size Size { get; set; }

        public override string ToString()
        {
            return $"{Size} {Flavor} Shake";
        }
    }
}

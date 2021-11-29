namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;
    using System.Text;


    public class Burger
    {
        public Guid BurgerId { get; set; }
        public decimal Weight { get; set; } = 0.5m;
        public bool Lettuce { get; set; }
        public bool Cheese { get; set; }
        public bool Pickle { get; set; } = true;
        public bool Onion { get; set; } = true;
        public bool Ketchup { get; set; }
        public bool Mustard { get; set; } = true;
        public bool BarbecueSauce { get; set; }
        public bool OnionRing { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Burger: {0:F2} lb", Weight);

            if (Cheese)
                sb.Append(" Cheese");
            if (Lettuce)
                sb.Append(" Lettuce");
            if (Pickle)
                sb.Append(" Pickle");
            if (Onion)
                sb.Append(" Onion");
            if (Ketchup)
                sb.Append(" Ketchup");
            if (Mustard)
                sb.Append(" Mustard");
            if (BarbecueSauce)
                sb.Append(" BBQ");
            if (OnionRing)
                sb.Append(" OnionRing");

            return sb.ToString();
        }
    }
}

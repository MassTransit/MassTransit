namespace MassTransit.TestFramework.Courier
{
    public class OuterObjectImpl :
        OuterObject
    {
        public OuterObjectImpl(int intValue, string stringValue, decimal decimalValue)
        {
            IntValue = intValue;
            StringValue = stringValue;
            DecimalValue = decimalValue;
        }

        public int IntValue { get; private set; }
        public string StringValue { get; private set; }
        public decimal DecimalValue { get; private set; }
    }
}

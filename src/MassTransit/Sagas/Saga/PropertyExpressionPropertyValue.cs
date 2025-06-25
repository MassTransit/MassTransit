namespace MassTransit.Saga;

public class PropertyExpressionPropertyValue<TProperty> :
    IPropertyExpressionPropertyValue
{
    public TProperty Value { get; set; }

    public object GetValue()
    {
        return Value;
    }
}

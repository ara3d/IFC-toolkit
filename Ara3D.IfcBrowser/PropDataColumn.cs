using System.ComponentModel;

namespace Ara3D.IfcBrowser;

public class PropDataColumn : PropertyDescriptor
{
    public int ColumnIndex;
    public Type Type;
    public PropData Data;

    public PropDataColumn(PropData data, int columnIndex, string name, Type type)
        : base(name, null)
    {
        Data = data;
        ColumnIndex = columnIndex;
        Type = type;
    }

    public override bool CanResetValue(object component)
        => false;

    public override object GetValue(object component)
    {
        if (component is not PropDataRow pdr)
            throw new ArgumentException("Incorrect component type", nameof(component));
        return pdr.GetValue(ColumnIndex);
    }

    public override void ResetValue(object component)
        => throw new NotImplementedException();

    public override void SetValue(object component, object value)
        => throw new NotImplementedException();

    public override bool ShouldSerializeValue(object component)
        => false;

    public override Type ComponentType
        => typeof(PropDataRow);

    public override bool IsReadOnly
        => true;

    public override Type PropertyType
        => Type;
        
}
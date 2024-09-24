using System.Collections;
using System.ComponentModel;
using System.Data;
using Ara3D.IfcPropDB;

namespace Ara3D.IfcBrowser;

public class PropDataRow : ICustomTypeDescriptor, IEditableObject, INotifyPropertyChanged, IEnumerator
{
    public int RowIndex;
    public PropData Data;
    public IfcPropertyDataReader.PropVal Val;
    public IfcPropertyDataReader.PropDesc Desc;

    public PropDataRow(PropData data, int rowIndex = -1)
    {
        RowIndex = rowIndex;
        Data = data;
        Val = data.Source.PropValTable[RowIndex];
        Desc = data.Source.PropDescTable[Val.PropDescIndex];
    }

    public object GetValue(int index)
        => Data.GetValue(this, index);

    public AttributeCollection GetAttributes()
        => new(null);

    public string GetClassName()
        => null;

    public string GetComponentName()
        => null;

    public TypeConverter GetConverter()
        => null;

    public EventDescriptor GetDefaultEvent()
        => null;

    public PropertyDescriptor GetDefaultProperty()
        => null;

    public object GetEditor(Type editorBaseType)
        => null;

    public EventDescriptorCollection GetEvents()
        => new(null);

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
        => new(null);

    public PropertyDescriptorCollection GetProperties()
        => GetProperties(null);

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        => Data.GetItemProperties(null);

    public object GetPropertyOwner(PropertyDescriptor pd)
        => this;

    public void BeginEdit()
        => throw new ReadOnlyException();

    public void CancelEdit()
        => throw new ReadOnlyException();

    public void EndEdit()
        => throw new ReadOnlyException();

    public event PropertyChangedEventHandler PropertyChanged;

    public int NumRows
        => Data.NumRows;

    public bool MoveNext()
    {
        if (RowIndex + 1 >= NumRows)
            return false;
        RowIndex++;
        return true;
    }

    public void Reset()
        => RowIndex = -1;

    public object Current
        => this;
}
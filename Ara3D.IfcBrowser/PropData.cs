using System.Collections;
using System.ComponentModel;
using System.Data;
using Ara3D.IfcPropDB;

namespace Ara3D.IfcBrowser
{
    public class PropData : IBindingListView,
        ISupportInitializeNotification,
        ITypedList
    {
        public PropData(IfcPropertyDataReader reader)
        {
            Source = reader;
            Count = reader.PropValTable.Count;
            
            Columns = new() 
            {
                new PropDataColumn(this, 0, "Index", typeof(int)),
                new PropDataColumn(this, 1, "Value", typeof(string)),
                new PropDataColumn(this, 2, "Descriptor", typeof(int)),
                new PropDataColumn(this, 3, "Name", typeof(string)),
                new PropDataColumn(this, 4, "Entity", typeof(string)),
                new PropDataColumn(this, 5, "Unit", typeof(string)),
                new PropDataColumn(this, 6, "Description", typeof(string))
            };

            DataTable = new DataTable { TableName = Name };
            foreach (var c in Columns)
                DataTable.Columns.Add(c.Name, c.Type);
        }

        public object GetValue(PropDataRow row, int index)
        {
            switch (index)
            {
                case 0: return row.RowIndex;
                case 1: return row.Val.GetValue(Source.Strings);
                case 2: return row.Val.PropDescIndex;
                case 3: return row.Desc.GetName(Source.Strings);
                case 4: return row.Desc.GetEntity(Source.Strings);
                case 5: return row.Desc.GetUnit(Source.Strings);
                case 6: return row.Desc.GetDescription(Source.Strings);
            }
            return null;
        }

        public IfcPropertyDataReader Source { get; }
        public DataTable DataTable;
        public string Name => "IFC Property Data";
        public int Count { get; }
        public int NumRows => Count;
        public int NumColumns => Columns.Count;

        public List<PropDataColumn> Columns { get; }

        public PropDataRow GetRow(int row)
            => new(this, row);

        public int GetColumnIndex(string columnName)
        {
            for (var i = 0; i < Columns.Count; i++)
                if (Columns[i].Name == columnName)
                    return i;
            return -1;
        }

        public IEnumerator GetEnumerator()
            => new PropDataRow(this);

        public void CopyTo(Array array, int index)
        {
            for (var i = 0; i < Count; i++)
                array.SetValue(GetRow(i), i + index);
        }

        public bool IsSynchronized
            => true;

        public object SyncRoot
            => DataTable;

        public int Add(object value)
            => throw new NotImplementedException();

        public void Clear()
            => throw new NotImplementedException();

        public bool Contains(object value)
            => value is PropDataRow;

        public int IndexOf(object value)
            => value is PropDataRow pdr ? pdr.RowIndex : -1;

        public void Insert(int index, object value)
            => throw new NotImplementedException();

        public void Remove(object value)
            => throw new NotImplementedException();

        public void RemoveAt(int index)
            => throw new NotImplementedException();

        public bool IsFixedSize
            => true;

        public bool IsReadOnly
            => true;

        public object this[int index]
        {
            get => GetRow(index);
            set => throw new ReadOnlyException();
        }

        public void AddIndex(PropertyDescriptor property)
            => throw new NotImplementedException();

        public object AddNew()
            => throw new NotImplementedException();

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
            => throw new NotImplementedException();

        public int Find(PropertyDescriptor property, object key)
            => throw new NotImplementedException();

        public void RemoveIndex(PropertyDescriptor property)
            => throw new NotImplementedException();

        public void RemoveSort()
            => throw new NotImplementedException();

        public bool AllowEdit => false;
        public bool AllowNew => false;
        public bool AllowRemove => false;

        public bool IsSorted => false;

        public ListSortDirection SortDirection => ListSortDirection.Ascending;
        public PropertyDescriptor SortProperty => null;

        public bool SupportsChangeNotification => false;
        public bool SupportsSearching => false;
        public bool SupportsSorting => false;

        public event ListChangedEventHandler ListChanged;

        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            // Does nothing
        }

        public void RemoveFilter()
            => throw new NotImplementedException();

        public string Filter { get; set; }
        public ListSortDescriptionCollection SortDescriptions { get; } = new();
        public bool SupportsAdvancedSorting => false;
        public bool SupportsFiltering => false;

        public void BeginInit()
        {
        }

        public void EndInit()
        {
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        public bool IsInitialized { get; private set; }

        public event EventHandler Initialized;

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            => new(Columns.Cast<PropertyDescriptor>().ToArray(), true);

        public string GetListName(PropertyDescriptor[] listAccessors)
            => Name;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace SlySoft.RestResource.Client.Accessors;

public class EditableAccessorList<T> : EditableAccessor, IList<T>, INotifyCollectionChanged {
    private readonly IList<T> _sourceList;
    private ObservableCollection<T> _editList;

    public EditableAccessorList(IList<T> sourceList) {
        _sourceList = sourceList;
        _editList = new ObservableCollection<T>(sourceList);
        _editList.CollectionChanged += EditListCollectionChanged;

        foreach (var item in sourceList.OfType<EditableAccessor>()) {
            item.PropertyChanged += (_, _) => {
                RefreshIsChanged();
            };
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private void EditListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        CollectionChanged?.Invoke(this, e);
    }

    private void RefreshIsChanged() {
        IsChanged = _sourceList.Except(_editList).Any() 
                 || _editList.Except(_sourceList).Any()
                 || _sourceList.OfType<EditableAccessor>().Any(x => x.IsChanged);
    }

    public override void RejectChanges() {
        _editList.CollectionChanged -= EditListCollectionChanged;
        _editList = new ObservableCollection<T>(_sourceList);
        foreach (var item in _sourceList.OfType<EditableAccessor>()) {
            item.RejectChanges();
        }
        _editList.CollectionChanged += EditListCollectionChanged;
        IsChanged = false;
    }

    public IEnumerator<T> GetEnumerator() {
        return _editList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return _editList.GetEnumerator();
    }

    public void Add(T item) {
        _editList.Add(item);
        RefreshIsChanged();
    }

    public void Clear() {
        _editList.Clear();
        RefreshIsChanged();
    }

    public bool Contains(T item) {
        return _editList.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        _editList.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item) {
        var removed = _editList.Remove(item);
        RefreshIsChanged();
        return removed;
    }

    public int Count => _editList.Count;
    public bool IsReadOnly => false;
    public int IndexOf(T item) {
        return _editList.IndexOf(item);
    }

    public void Insert(int index, T item) {
        _editList.Insert(index, item);
        RefreshIsChanged();
    }

    public void RemoveAt(int index) {
        _editList.RemoveAt(index);
        RefreshIsChanged();
    }

    public T this[int index] {
        get => _editList[index];
        set {
            _editList[index] = value;
            RefreshIsChanged();
        }
    }
}
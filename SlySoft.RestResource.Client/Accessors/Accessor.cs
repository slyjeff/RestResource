using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace SlySoft.RestResource.Client.Accessors;

public abstract class Accessor : EditableAccessor {
    protected readonly ObjectData CachedData = new ();
    protected readonly ObjectData UpdateValues = new();
    private readonly IList<IEditableAccessor> _editableAccessors = new List<IEditableAccessor>();

    public override void RejectChanges() {
        foreach (var editableAccessor in _editableAccessors) {
            editableAccessor.RejectChanges();
        }

        if (UpdateValues.Any()) {
            var revertedProperties = UpdateValues.Keys.ToList();
            UpdateValues.Clear();
            foreach (var revertedProperty in revertedProperties) {
                OnPropertyChanged(revertedProperty);
            }
        }
        RefreshIsChanged();
    }

    protected T? GetData<T>(string name) {
        if (UpdateValues.TryGetValue(name, out var value)) {
            return (T?)value;
        }

        return GetOriginalData<T>(name);
    }

    protected void SetData<T>(string name, T? value) {
        //if this is the original value, remove our Update Value
        var originalValue = GetOriginalData<T>(name);
        if (ValuesAreEqual(originalValue, value)) {
            if (UpdateValues.ContainsKey(name)) {
                UpdateValues.Remove(name);
            }
        } else {
            UpdateValues[name] = value;
        }
        OnPropertyChanged(name);

        RefreshIsChanged();
    }

    private T? GetOriginalData<T>(string name) {
        if (CachedData.TryGetValue(name, out var value)) {
            return (T?)value;
        }
        
        var newData = CreateData<T>(name);

        if (newData is IEditableAccessor editableAccessor) {
            _editableAccessors.Add(editableAccessor);
            editableAccessor.PropertyChanged += (_, _) => {
                RefreshIsChanged();
            };
        }

        CachedData[name] = newData;

        return (T?)CachedData[name];
    }

    protected abstract T? CreateData<T>(string name);

    private static bool ValuesAreEqual<T>(T? v1, T? v2) {
        if (v1 == null && v2 == null) {
            return true;
        }

        return v1 != null && v1.Equals(v2);
    }

    private void RefreshIsChanged() {
        if (UpdateValues.Count > 0) {
            IsChanged = true;
            return;
        }

        if (_editableAccessors.Any(editableAccessor => editableAccessor.IsChanged)) {
            IsChanged = true;
            return;
        }

        IsChanged = false;
    }
}
using System.ComponentModel;

namespace SlySoft.RestResource.Client.Accessors;

/// Inheriting from IEditableAccessor allows a resource accessor to report changes to properties, whether the resource has changes, and revert changes
public interface IEditableAccessor : INotifyPropertyChanged {
    /// <summary>
    /// Whether changes have been made to any properties in the resource
    /// </summary>
    bool IsChanged { get; }

    /// <summary>
    /// Revert changes back to the original values received from the service call
    /// </summary>
    void RejectChanges();
}

public abstract class EditableAccessor : IEditableAccessor {
    private bool _isChanged;
    public bool IsChanged {
        get => _isChanged;
        set {
            if (value == _isChanged) {
                return;
            }

            _isChanged = value;
            OnPropertyChanged(nameof(IsChanged));
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public abstract void RejectChanges();
}
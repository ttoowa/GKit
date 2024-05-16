using System;

namespace GKit;

public interface IRuntimeValue {
    public event Action ValueChangedSimple;
    
    public void InvokeValueChanged();
}

public class RuntimeValue<T> : IRuntimeValue {
    public T Value {
        get => value;
        set {
            this.value = value;
            InvokeValueChanged();
        }
    }
    
    private T value;
    public event Action<T> ValueChanged;
    public event Action ValueChangedSimple;
    
    public RuntimeValue() {
        value = default;
    }
    
    public RuntimeValue(T value) {
        this.value = value;
    }
    
    public void InvokeValueChanged() {
        ValueChanged?.Invoke(value);
        ValueChangedSimple?.Invoke();
    }
}
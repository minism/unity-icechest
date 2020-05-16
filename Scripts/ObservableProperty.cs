namespace Ice {

  public class ObservableProperty<T> {
    private T _value;
    public T Value {
      get { return _value; }
      set {
        if (!value.Equals(Value)) {
          Value = value;
          OnChange?.Invoke(Value);
        }
      }
    }

    public delegate void ChangeEvent(T data);
    public event ChangeEvent OnChange;

    public ObservableProperty(T initialValue) {
      Value = initialValue;
    }

    public static implicit operator T(ObservableProperty<T> p) {
      return p.Value;
    }
  }

} // namespace Ice

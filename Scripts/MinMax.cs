namespace Ice {

  public class MinMax<T> where T : System.IComparable {
    public T Max { get; private set; }
    public T Min { get; private set; }

    private bool maxSet, minSet;

    public void Add(T value) {
      if (!maxSet || value.CompareTo(Max) > 0) {
        Max = value;
        maxSet = true;
      }
      if (!minSet || value.CompareTo(Min) < 0) {
        Min = value;
        minSet = true;
      }
    }
  }

} // namespace Ice

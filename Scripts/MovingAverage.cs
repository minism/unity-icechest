using System;
using System.Linq;
using CircularBuffer;

namespace Ice {

  public class MovingAverage {
    private CircularBuffer<float> buffer;

    public MovingAverage(int windowSize) {
      buffer = new CircularBuffer<float>(windowSize);
    }

    public void Push(float value) {
      buffer.PushFront(value);
    }

    public float Average() {
      return buffer.Sum() / buffer.Count();
    }
  }

} // namespace Ice

using System.Collections;
using System.Collections.Generic;

namespace Ice {
  public static class MathUtil {
    public static int RoundToMultiple(int value, int multiple) {
      return (value / multiple) * multiple;
    }

    public static int RoundToMultiple(float value, int multiple) {
      return RoundToMultiple((int)value, multiple);
    }
  }
}

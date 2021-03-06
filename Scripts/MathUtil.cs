using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {
  public static class MathUtil {
    public static int RoundToMultiple(int value, int multiple) {
      return (value / multiple) * multiple;
    }

    public static int RoundToMultiple(float value, int multiple) {
      return RoundToMultiple(Mathf.FloorToInt(value), multiple);
    }

    public static Vector3 RoundToMultiple(Vector3 vector, int multiple) {
      return new Vector3(
          RoundToMultiple(vector.x, multiple),
          RoundToMultiple(vector.y, multiple),
          RoundToMultiple(vector.z, multiple));
    }

    public static Vector3 Round(Vector3 vector) {
      return new Vector3(
        Mathf.Round(vector.x),
        Mathf.Round(vector.y),
        Mathf.Round(vector.z));
    }
  }
}

using UnityEngine;

namespace Ice {
  public static class ObjectUtil {
    public static void DestroyChildrenImmediate(GameObject parent) {
      GameObject[] children = new GameObject[parent.transform.childCount];
      for (int i = 0; i < parent.transform.childCount; i++) {
        children[i] = parent.transform.GetChild(i).gameObject;
      }
      foreach (var child in children) {
        Object.DestroyImmediate(child.gameObject);
      }
    }
  }
} // namespace Ice

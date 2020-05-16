using UnityEngine;

namespace Ice {
  public static class ObjectUtil {
    public static void DestroyChildrenImmediate(GameObject parent) {
      DestroyChildrenImmediate(parent.transform);
    }

    public static void DestroyChildrenImmediate(Transform parent) {
      GameObject[] children = new GameObject[parent.childCount];
      for (int i = 0; i < parent.childCount; i++) {
        children[i] = parent.GetChild(i).gameObject;
      }
      foreach (var child in children) {
        Object.DestroyImmediate(child.gameObject);
      }
    }

    public static void DestroyChildren(GameObject parent) {
      DestroyChildren(parent.transform);
    }

    public static void DestroyChildren(Transform parent) {
      foreach (Transform child in parent) {
        Object.Destroy(child.gameObject);
      }
    }

    public static GameObject FindChildWithTag(GameObject parent, string tag) {
      return FindChildWithTag(parent.transform, tag);
    }

    public static GameObject FindChildWithTag(Transform parent, string tag) {
      foreach (Transform child in parent) {
        if (child.CompareTag(tag)) {
          return child.gameObject;
        }
      }
      return null;
    }
  }
} // namespace Ice

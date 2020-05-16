using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Ice {

  /// Adds method-invoking buttons to component inspectors for any methods that are tagged with
  /// "ExposeMethod"
  [CanEditMultipleObjects]
  [CustomEditor(typeof(MonoBehaviour), true)]
  public class MonoBehaviorCustomEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
      DrawDefaultInspector();

      var type = target.GetType();
      foreach (var method in type.GetMethods(
                 BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
        var attributes = method.GetCustomAttributes(typeof(ExposeMethodAttribute), true);
        if (attributes.Length > 0) {
          if (GUILayout.Button("Run: " + method.Name)) {
            method.Invoke(target, null);
          }
        }
      }
    }
  }

} // namespace Ice

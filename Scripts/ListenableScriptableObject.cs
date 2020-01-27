using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  public class ListenableScriptableObject : ScriptableObject {
    public event System.Action OnChanged;

    [Tooltip("Whether to automatically notify listeners when modifying values in editor.")]
    public bool updateListenersEditor;

    private void OnValidate() {
      if (updateListenersEditor) {
        NotifyChanged();
      }
    }

    // Notify all listeners that we've changed values.
    public void NotifyChanged() {
      OnChanged?.Invoke();
    }
  }

  #if UNITY_EDITOR

  [UnityEditor.CustomEditor(typeof(ListenableScriptableObject), true)]
  public class ListenableScriptableObjectEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();
      var listenable = (ListenableScriptableObject) target;
      if (GUILayout.Button("Notify changed")) {
        listenable.NotifyChanged();
      }
    }
  }

  #endif

} // namespace Ice

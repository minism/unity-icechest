using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

/// Simple utility for wrapping GetComponent and caching the result.
/// The only thing this really provides beyond doing it in Start is that it would
/// work for Editor-time logic as well, which can be an annoyance.
public class CachedComponent<T> where T : Component {
  private MonoBehaviour behaviour;
  private T component;

  public CachedComponent(MonoBehaviour behaviour) {
    this.behaviour = behaviour;
  }

  public T Get() {
    if (component == null) {
      component = behaviour.GetComponent<T>();
    }
    return component;
  }
}

public static class CachedComponent {
  private static System.Type cachedComponentType = typeof(CachedComponent<>);

  public static void Init(MonoBehaviour behaviour) {
    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    var fields = behaviour.GetType().GetFields(flags);
    foreach (var field in fields) {
      if (field.FieldType.IsGenericType &&
          field.FieldType.GetGenericTypeDefinition() == cachedComponentType) {
        var componentType = field.FieldType.GetGenericArguments()[0];
        var finalType = cachedComponentType.MakeGenericType(componentType);
        var ctor = finalType.GetConstructor(new[] {typeof(MonoBehaviour) });
        field.SetValue(behaviour, ctor.Invoke(new[] { behaviour }));
      }
    }
  }
}

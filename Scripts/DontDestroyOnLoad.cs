using UnityEngine;

namespace Ice {
  public class DontDestroyOnLoad : MonoBehaviour {
    private void Awake() {
      DontDestroyOnLoad(gameObject);
    }
  }
} // namespace Ice

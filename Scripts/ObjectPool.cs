using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Manages a dynamic pool of objects, to improve performance when many instantiations are
/// occuring per-frame.
namespace Ice {

  public class ObjectPool : MonoBehaviour {
    public struct PoolType {
      public string tag;
      public GameObject prefab;
      public int poolSize;
    }
    public List<PoolType> poolTypes;

    // TODO: IMPLEMENT COLD START.
    public bool prewarm = true;

    private Dictionary<string, Queue<GameObject>> pools;

    // Use this for initialization
    void Start() {
      pools = new Dictionary<string, Queue<GameObject>>();
      foreach (PoolType poolType in poolTypes) {
        var pool = new Queue<GameObject>();
        for (int i = 0; i < poolType.poolSize; ++i) {
          var obj = GameObject.Instantiate(poolType.prefab);
          obj.SetActive(false);
          pool.Enqueue(obj);
        }
      }
    }

    public GameObject Get(string tag) {
      return Get(tag, Vector3.zero);
    }

    public GameObject Get(string tag, Vector3 position) {
      return Get(tag, position, Quaternion.identity);
    }

    public GameObject Get(string tag, Vector3 position, Quaternion rotation) {
      var obj = pools[tag].Dequeue();
      obj.SetActive(true);
      obj.transform.position = position;
      obj.transform.rotation = rotation;
      pools[tag].Enqueue(obj);

      // Notify the object that its been spawned rather than created fresh.
      foreach (var poolableComponent in obj.GetComponents<IPoolable>()) {
        // TODO: Can we just use OnEnable instead here?
        poolableComponent.OnSpawn();
      }

      return obj;
    }
  }

} // namespace Ice

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple component which automatically destroys a gameobject after a time interval.
namespace Ice {

  public class Lifetime : MonoBehaviour {

    public float lifetime;
    public float TimeLeft {
      get {
        return Time.time - startTime;
      }
    }

    private float startTime;

    void Start() {
      startTime = Time.time;
    }

    void Update() {
      if (lifetime > 0 && TimeLeft > lifetime) {
        Destroy(gameObject);
      }
    }
  }

} // namespace Ice

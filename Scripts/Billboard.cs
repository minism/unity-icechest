using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  public class Billboard : MonoBehaviour {
    public Camera cameraToFace; // Default to main.
    public bool flip;

    private void Start() {
      if (cameraToFace == null) {
        cameraToFace = Camera.main;
      }
    }

    private void Update() {
      var target = cameraToFace.transform;
      var position = flip ? transform.position * 2 - target.position : target.position;
      transform.LookAt(position);
    }
  }

} // namespace Ice

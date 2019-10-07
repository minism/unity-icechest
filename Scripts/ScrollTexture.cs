using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// Simple class to scroll texture offsets.
  public class ScrollTexture : MonoBehaviour {
    public float xSpeed, ySpeed;

    private Material material;
    private float offsetX, offsetY;

    private void Start() {
      var renderer = GetComponent<Renderer>();
      if (renderer != null) {
        material = renderer.sharedMaterial;
      }
      if (material == null) {
        Debug.LogWarning("No renderer or material to scroll.");
      }
    }

    private void Update() {
      if (material == null) {
        return;
      }
      offsetX = (offsetX + xSpeed * Time.deltaTime) % 1;
      offsetY = (offsetY + ySpeed * Time.deltaTime) % 1;
      material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
  }

} // namespcae Ice

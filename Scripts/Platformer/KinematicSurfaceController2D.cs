using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// Kinematic moving surfaces that characters interact with, like moving platforms.
  /// TODO: This is incomplete.
  public class KinematicSurfaceController2D : RaycastController2D {

    public Vector3 move;

    private Vector3 velocity;

    protected override void Start() {
      base.Start();
    }

    private void Update() {
      Vector3 velocity = move * Time.deltaTime;
      transform.Translate(velocity);
    }

    protected void MovePassengers() {
      float dirX = Mathf.Sign(velocity.x);
      float dirY = Mathf.Sign(velocity.y);

      // Surface is moving vertically.
      if (velocity.y != 0) {
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < vertRayCount; i++) {
          Vector2 ro = dirY < 0 ? rayOrigins.bottomLeft : rayOrigins.topLeft;
          ro += Vector2.right * (vertRaySpacing * i + velocity.x);

          var hit = Physics2D.Raycast(ro, Vector2.up * dirY, rayLength, collisionMask);
        }
      }
    }
  }

} // namespace Ice

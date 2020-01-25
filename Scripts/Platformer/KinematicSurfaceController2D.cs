using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// Kinematic moving surfaces that characters interact with, like moving platforms.
  /// TODO: This is incomplete.
  public class KinematicSurfaceController2D : RaycastController2D {

    public Vector2 oscillationVector;
    public float oscillationSpeed;

    private Vector3 initialPosition;
    private HashSet<Transform> passengersMoved = new HashSet<Transform>();

    protected override void Start() {
      base.Start();

      initialPosition = transform.position;
    }

    private void Update() {
      var t = Mathf.Sin(Time.time * oscillationSpeed);
      var nextPosition = initialPosition + (Vector3)oscillationVector * t;
      var velocity = nextPosition - transform.position;
      MovePassengers(velocity);
      transform.Translate(velocity);
    }

    private void MovePassengers(Vector3 velocity) {
      UpdateRayOrigins();
      passengersMoved.Clear();
      float dirX = Mathf.Sign(velocity.x);
      float dirY = Mathf.Sign(velocity.y);

      // Surface is moving vertically.
      if (velocity.y != 0) {
        foreach (var hit in CheckVerticalCollisions(velocity, false)) {
          if (!passengersMoved.Contains(hit.transform)) {
            float pushY = velocity.y - (hit.distance - skinWidth) * dirY;
            float pushX = dirY == 1 ? velocity.x : 0;
            hit.transform.Translate(new Vector3(pushX, pushY));
            passengersMoved.Add(hit.transform);
          }
        }
      }

      // Surface is moving horizontally.
      if (velocity.x != 0) {
        foreach (var hit in CheckHorizontalCollisions(velocity, false)) {
          if (!passengersMoved.Contains(hit.transform)) {
            float pushX = velocity.x - (hit.distance - skinWidth) * dirY;
            float pushY = 0;
            hit.transform.Translate(new Vector3(pushX, pushY));
            passengersMoved.Add(hit.transform);
          }
        }
      }

      // Check for passengers on top of horizontally or downward moving platform.
      // Upward raycast check only.
      if (dirY == -1 || velocity.y == 0 && velocity.x != 0) {
        float rayLength = skinWidth * 2;
        for (int i = 0; i < vertRayCount; i++) {
          Vector2 ro = rayOrigins.topLeft;
          ro += Vector2.right * vertRaySpacing * i;
          var hit = Physics2D.Raycast(ro, Vector2.up, rayLength, collisionMask);
          if (hit && !passengersMoved.Contains(hit.transform)) {
            float pushX = velocity.x;
            float pushY = velocity.y;
            hit.transform.Translate(new Vector3(pushX, pushY));
            passengersMoved.Add(hit.transform);
          }
        }

      }
    }
  }

} // namespace Ice

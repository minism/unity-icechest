using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// Kinematic moving surfaces that characters interact with, like moving platforms.
  /// TODO: This is incomplete.
  public class KinematicSurfaceController2D : RaycastController2D {

    public Vector2 oscillationVector;
    public float oscillationSpeed;
    public bool onlyMoveControllers;

    private Vector3 initialPosition;

    private struct PassengerMovement {
      public Transform transform;
      public Vector3 velocity;
      public bool standingOnPlatform;
      public bool passengerMovesBeforeUs;
    }

    protected override void Start() {
      base.Start();

      initialPosition = transform.position;
    }

    private void Update() {
      var t = Mathf.Sin(Time.time * oscillationSpeed);
      var nextPosition = initialPosition + (Vector3)oscillationVector * t;
      var velocity = nextPosition - transform.position;
      
      // Move all passengers.
      var movements = PreparePassengerMovements(velocity);
      MovePassengers(movements, true);
      transform.Translate(velocity);
      MovePassengers(movements, false);
    }

    private void MovePassengers(IEnumerable<PassengerMovement> movements, bool beforeWeMoved) {
      foreach (var m in movements.Where(m => m.passengerMovesBeforeUs == beforeWeMoved).ToList()) {
        // TODO: Dictionary to cache this per-passenger object.
        var controller = m.transform.GetComponent<PlatformerController2D>();
        if (controller != null) {
          controller.Move(m.velocity, m.standingOnPlatform);
        } else if (!onlyMoveControllers) {
          // If there's no platformer controller, just move the object directly.
          m.transform.Translate(m.velocity);
        }
      }
    }

    private IEnumerable<PassengerMovement> PreparePassengerMovements(Vector3 velocity) {
      List<PassengerMovement> movements = new List<PassengerMovement>();

      UpdateRayOrigins();
      var passengersVisited = new HashSet<Transform>();
      float dirY = Mathf.Sign(velocity.y);

      // Surface is moving vertically.
      if (velocity.y != 0) {
        foreach (var hit in CheckVerticalCollisions(velocity, false)) {
          if (!passengersVisited.Contains(hit.transform)) {
            float pushY = velocity.y - (hit.distance - skinWidth) * dirY;
            float pushX = dirY == 1 ? velocity.x : 0;
            movements.Add(new PassengerMovement {
              transform = hit.transform,
              velocity = new Vector3(pushX, pushY),
              standingOnPlatform = dirY == 1,
              passengerMovesBeforeUs = true,
            });
            passengersVisited.Add(hit.transform);
          }
        }
      }

      // Surface is moving horizontally.
      if (velocity.x != 0) {
        foreach (var hit in CheckHorizontalCollisions(velocity, false)) {
          if (!passengersVisited.Contains(hit.transform)) {
            float pushX = velocity.x - (hit.distance - skinWidth) * dirY;
            float pushY = 0;
            movements.Add(new PassengerMovement {
              transform = hit.transform,
              velocity = new Vector3(pushX, pushY),
              standingOnPlatform = false,
              passengerMovesBeforeUs = true,
            });
            passengersVisited.Add(hit.transform);
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
          if (hit && !passengersVisited.Contains(hit.transform)) {
            float pushX = velocity.x;
            float pushY = velocity.y;
            movements.Add(new PassengerMovement {
              transform = hit.transform,
              velocity = new Vector3(pushX, pushY),
              standingOnPlatform = true,
              passengerMovesBeforeUs = false,
            });
            passengersVisited.Add(hit.transform);
          }
        }
      }

      return movements;
    }
  }

} // namespace Ice

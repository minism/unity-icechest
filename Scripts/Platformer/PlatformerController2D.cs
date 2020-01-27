using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// A raycast-based 2d platformer movement controller that doesn't rely on rigidbodies.
  /// Developed by following Seabstian Lague's tutorial here:
  /// https://www.youtube.com/watch?v=MbWK8bCAU2w
  public class PlatformerController2D : RaycastController2D {
    public float maxClimbAngle = 60f;
    public float maxDescendAngle = 45f;

    private CollisionData collisions;

    public CollisionData Collisions {
      get {
        return collisions;
      }
    }

    // TODO: See if this can be converted to a vector2 input.
    public void Move(Vector3 velocity, bool standingOnSurface = false) {
      UpdateRayOrigins();
      collisions.Reset();
      if (velocity.y < 0) {
        MaybeDescendSlope(ref velocity);
      }
      if (velocity.x != 0) {
        HandleHorizontalCollisions(ref velocity);
      }
      if (velocity.y != 0) {
        HandleVerticalCollisions(ref velocity);
      }
      transform.Translate(velocity);

      // External objects like moving platforms can simply inform us we're standing on them instead
      // of us needing to calculate it based on input + gravity, so we can always jump.
      if (standingOnSurface) {
        collisions.below = true;
      }
    }

    private void HandleHorizontalCollisions(ref Vector3 velocity) {
      float dirX = Mathf.Sign(velocity.x);
      float rayLength = Mathf.Abs(velocity.x) + skinWidth;
      for (int i = 0; i < horizRayCount; i++) {
        Vector2 ro = dirX < 0 ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
        ro += Vector2.up * (horizRaySpacing * i);

        var hit = Physics2D.Raycast(ro, Vector2.right * dirX, rayLength, collisionMask);
        if (hit) {
          float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

          // Check slopes on first ray only (foot).
          if (i == 0 && slopeAngle <= maxClimbAngle) {
            float slopeDistance = 0;
            if (slopeAngle != collisions.lastSlopeAngle) {
              // New slope encountered.
              slopeDistance = hit.distance - skinWidth;
              velocity.x -= slopeDistance * dirX;
            }
            ClimbSlope(ref velocity, slopeAngle);
            velocity.x += slopeDistance * dirX;
          }

          // Update velocity from ray.
          if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
            collisions.left = dirX < 0;
            collisions.right = dirX > 0;
            velocity.x = (hit.distance - skinWidth) * dirX;
            rayLength = hit.distance; // Update ray length so shortest ray in the loop wins.

            // If we collide horizontally, fix the Y vector based on the slope angle.
            if (collisions.climbingSlope) {
              velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
            }
          }
        }
      }
    }

    private void HandleVerticalCollisions(ref Vector3 velocity) {
      float dirY = Mathf.Sign(velocity.y);
      foreach (var hit in CheckVerticalCollisions(velocity)) {
        collisions.below = dirY < 0;
        collisions.above = dirY > 0;
        velocity.y = (hit.distance - skinWidth) * dirY;

        // If we collide vertically, fix the X vector based on the slope angle.
        if (collisions.climbingSlope) {
          velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
        }
      }

      // Check for slope-to-slope angle changes.
      if (collisions.climbingSlope) {
        float dirX = Mathf.Sign(velocity.x);
        float rayLengthX = Mathf.Abs(velocity.x) + skinWidth;
        Vector2 ro = dirX < 0 ? rayOrigins.bottomLeft : rayOrigins.bottomRight + Vector2.up * velocity.y;
        var hit = Physics2D.Raycast(ro, Vector2.right * dirX, rayLengthX, collisionMask);
        if (hit) {
          float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
          if (slopeAngle != collisions.slopeAngle) {
            // New slope encountered.
            velocity.x = (hit.distance - skinWidth) * dirX;
            collisions.slopeAngle = slopeAngle;
          }
        }
      }
    }

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
      float moveDist = Mathf.Abs(velocity.x);
      float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
      if (velocity.y <= climbVelocityY) {
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
        velocity.y = climbVelocityY;
        collisions.below = true; // Slope always counts as grounded.
        collisions.climbingSlope = true;
        collisions.slopeAngle = slopeAngle;
      }
    }

    private void MaybeDescendSlope(ref Vector3 velocity) {
      float dirX = Mathf.Sign(velocity.x);

      // Cast a ray from the back foot.
      Vector2 ro = dirX < 0 ? rayOrigins.bottomRight : rayOrigins.bottomLeft;
      var hit = Physics2D.Raycast(ro, -Vector2.up, Mathf.Infinity, collisionMask);
      if (hit) {
        float descendSlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (descendSlopeAngle != 0 &&                 // The angle is non-zero.
            descendSlopeAngle <= maxDescendAngle &&   // This is a new slope.
            Mathf.Sign(hit.normal.x) == dirX &&       // We're traveling down the slope.
            hit.distance - skinWidth <=               // We will collide with the slope.
                Mathf.Tan(descendSlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
          float moveDist = Mathf.Abs(velocity.x);
          float descendVelocityY = Mathf.Sin(descendSlopeAngle * Mathf.Deg2Rad) * moveDist;
          velocity.x = Mathf.Cos(descendSlopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
          velocity.y -= descendVelocityY;

          // Update collisions.
          collisions.below = true; // Slope always counts as grounded.
          collisions.descendingSlope = true;
          collisions.slopeAngle = descendSlopeAngle;
        }
      }
    }
  }

  public struct CollisionData {
    public bool above, below, left, right;
    public bool climbingSlope, descendingSlope;
    public float slopeAngle, lastSlopeAngle;

    public void Reset() {
      above = below = left = right = false;
      climbingSlope = descendingSlope = false;
      lastSlopeAngle = slopeAngle;
      slopeAngle = 0;
    }
  }

} //namespace Ice

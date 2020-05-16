using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// Base component class for 2D raycast-based movement controllers.
  [RequireComponent(typeof(BoxCollider2D))]
  public abstract class RaycastController2D : MonoBehaviour {
    public float skinWidth = 0.015f;
    public int horizRayCount = 4;
    public int vertRayCount = 4;
    public LayerMask collisionMask;

    protected BoxCollider2D collider;
    protected RaycastOrigins rayOrigins;
    protected float horizRaySpacing;
    protected float vertRaySpacing;

    virtual protected void Start() {
      collider = GetComponent<BoxCollider2D>();
      UpdateRaySpacing();
    }

    protected IEnumerable<RaycastHit2D> CheckVerticalCollisions(Vector3 velocity) {
      return CheckVerticalCollisions(velocity, true);
    }

    protected IEnumerable<RaycastHit2D> CheckVerticalCollisions(
        Vector3 velocity, bool projectVelocity) {
      float directionY = Mathf.Sign(velocity.y);
      float rayLength = Mathf.Abs(velocity.y) + skinWidth;
      for (int i = 0; i < vertRayCount; i++) {
        Vector2 ro = directionY < 0 ? rayOrigins.bottomLeft : rayOrigins.topLeft;
        ro += Vector2.right * (vertRaySpacing * i + (projectVelocity ? velocity.x : 0));
        Debug.DrawRay(ro, Vector2.up * directionY);
        var hit = Physics2D.Raycast(ro, Vector2.up * directionY, rayLength, collisionMask);
        if (hit) {
          // TODO(BUG): For moving platforms with multiple passengers, we should not short circuit here.
          rayLength = hit.distance; // Update ray length so shortest ray in the loop wins.
          yield return hit;
        }
      }
      yield break;
    }

    protected IEnumerable<RaycastHit2D> CheckHorizontalCollisions(
        Vector3 velocity, bool projectVelocity) {
      float directionX = Mathf.Sign(velocity.x);
      float rayLength = Mathf.Abs(velocity.x) + skinWidth;
      for (int i = 0; i < horizRayCount; i++) {
        Vector2 ro = directionX < 0 ? rayOrigins.bottomLeft : rayOrigins.bottomRight;
        ro += Vector2.up * (horizRaySpacing * i + (projectVelocity ? velocity.y : 0));
        Debug.DrawRay(ro, Vector2.right * directionX);
        var hit = Physics2D.Raycast(ro, Vector2.right * directionX, rayLength, collisionMask);
        if (hit) {
          rayLength = hit.distance; // Update ray length so shortest ray in the loop wins.
          yield return hit;
        }
      }
      yield break;
    }

    protected void UpdateRaySpacing() {
      var bounds = GetSkinBounds();

      horizRayCount = Mathf.Clamp(horizRayCount, 2, int.MaxValue);
      vertRayCount = Mathf.Clamp(vertRayCount, 2, int.MaxValue);
      horizRaySpacing = bounds.size.y / (horizRayCount - 1);
      vertRaySpacing = bounds.size.x / (vertRayCount - 1);
    }

    protected void UpdateRayOrigins() {
      var bounds = GetSkinBounds();

      rayOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
      rayOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
      rayOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
      rayOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private Bounds GetSkinBounds() {
      Bounds bounds = collider.bounds;
      bounds.Expand(skinWidth * -2);
      return bounds;
    }
  }

  public struct RaycastOrigins {
    public Vector2 topLeft, topRight, bottomLeft, bottomRight;
  }

} // namespace Ice

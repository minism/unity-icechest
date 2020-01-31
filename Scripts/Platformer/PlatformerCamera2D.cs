using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  // Platformer-style camera follower.
  public class PlatformerCamera2D : MonoBehaviour {
    public Transform target {
      get {
        return _target;
      }
      set {
        _target = value;
        EnsureTargetReferences();
      }
    }
    [SerializeField]
    private Transform _target;

    public Vector2 focusBoxSize = new Vector2(3, 3);
    public bool lockXAxis, lockYAxis;
    public float cameraHeight = 0;
    public float lookAheadDistance = 1;
    public float smoothTime = 0.2f;

    private FocusBox focusBox;
    private bool isLookingAhead;
    private Collider2D targetCollider;
    private IInputProvider targetInputProvider;

    // Lookahead state.
    private float currentLookAheadX, targetLookAheadX, lookAheadDirection;

    // Velocity smoothing state.
    private float smoothVelocityX, smoothVelocityY;

    private Vector3 initialPosition;

    // An input provider needed for lookahead functionality.
    public interface IInputProvider {
      Vector2 GetInputVector();
    }

    // Region surrounding the target which the camera keeps centered.
    // The target can move freely within this box before the camera itself moves.
    struct FocusBox {
      public Vector2 center;
      public Vector2 velocity;

      // TODO: Refactor using Bounds here.
      private float top, right, bottom, left;

      public FocusBox(Bounds targetBounds, Vector2 size) {
        left = targetBounds.center.x - size.x / 2;
        right = targetBounds.center.x + size.x / 2;
        bottom = targetBounds.min.y;  // Anchor to target's "feet".
        top = targetBounds.min.y + size.y;

        center = new Vector2((left + right) / 2, (top + bottom) / 2);
        velocity = Vector2.zero;
      }

      public void Update(Bounds targetBounds) {
        velocity = Vector2.zero;
        if (targetBounds.min.x < left) {
          velocity.x = targetBounds.min.x - left;
        } else if (targetBounds.max.x > right) {
          velocity.x = targetBounds.max.x - right;
        }
        if (targetBounds.min.y < bottom) {
          velocity.y = targetBounds.min.y - bottom;
        } else if (targetBounds.max.y > top) {
          velocity.y = targetBounds.max.y - top;
        }
        left += velocity.x;
        right += velocity.x;
        top += velocity.y;
        bottom += velocity.y;

        center = new Vector2((left + right) / 2, (top + bottom) / 2);
      }
    }

    private void Start() {
      initialPosition = transform.position;
      EnsureTargetReferences();
      focusBox = new FocusBox(targetCollider.bounds, focusBoxSize);
    }

    private void LateUpdate() {
      focusBox.Update(target.GetComponent<Collider2D>().bounds);
      var focalPoint = focusBox.center + Vector2.up * cameraHeight;
      
      Vector2 inputVector = Vector2.zero;
      if (targetInputProvider != null) {
        inputVector = targetInputProvider.GetInputVector();
      }

      // Handle lookahead - both player input and focus box must be moving in the same direction.
      if (focusBox.velocity.x != 0) {
        lookAheadDirection = Mathf.Sign(focusBox.velocity.x);
        var idealLookAheadX = lookAheadDirection * lookAheadDistance;
        if (Mathf.Sign(inputVector.x) == Mathf.Sign(focusBox.velocity.x) &&
            inputVector.x != 0) {
          isLookingAhead = true;
          targetLookAheadX = idealLookAheadX;
        } else if (isLookingAhead) {
          isLookingAhead = false;
          targetLookAheadX = currentLookAheadX + (idealLookAheadX - currentLookAheadX) / 4f;
        }
      }

      // Apply smoothing.
      currentLookAheadX = Mathf.SmoothDamp(
          currentLookAheadX, targetLookAheadX, ref smoothVelocityX, smoothTime);
      focalPoint.y = Mathf.SmoothDamp(
          transform.position.y, focalPoint.y, ref smoothVelocityY, smoothTime);
      focalPoint += Vector2.right * currentLookAheadX;

      // Set final camera position.
      var finalPosition = (Vector3)focalPoint + Vector3.forward * -10f;
      transform.position = new Vector3(
          lockXAxis ? initialPosition.x : finalPosition.x,
          lockYAxis ? initialPosition.y : finalPosition.y,
          finalPosition.z);
    }

    private void OnDrawGizmosSelected() {
      Gizmos.color = new Color(0, 0.8f, 0, 0.25f);
      Gizmos.DrawCube(focusBox.center, focusBoxSize);
    }

    private void EnsureTargetReferences() {
      targetCollider = target.GetComponent<Collider2D>();
      if (targetCollider == null) {
        // TODO: Camera should work with a bounds proxy size of zero.
        Debug.LogError("Camera target must have a collider or focus box won't work.");
      }
      targetInputProvider = target.GetComponent<IInputProvider>();
      if (targetInputProvider == null) {
        Debug.LogWarning("No input provider on the camera target, look-ahead may be jarring.");
      }
    }
  }

} // namespace Ice

using UnityEngine;

namespace Ice {

  public static class CameraUtil {
    public static Matrix4x4 GetFrustrumCorners(Camera cam) {
      float fov = cam.fieldOfView;
      float aspect = cam.aspect;
      float halfFov = fov / 2;
      float tanFov = Mathf.Tan(halfFov * Mathf.Deg2Rad);

      Vector3 distToRight = Vector3.right * tanFov * aspect;
      Vector3 distToTop = Vector3.up * tanFov;
      Vector3 center = -Vector3.forward;
      Vector3 topLeft = (center - distToRight + distToTop);
      Vector3 topRight = (center + distToRight + distToTop);
      Vector3 bottomRight = (center + distToRight - distToTop);
      Vector3 bottomLeft = (center - distToRight - distToTop);

      Matrix4x4 corners = Matrix4x4.identity;
      corners.SetRow(0, topLeft);
      corners.SetRow(1, topRight);
      corners.SetRow(2, bottomRight);
      corners.SetRow(3, bottomLeft);
      return corners;
    }
  }

} // namespace Ice

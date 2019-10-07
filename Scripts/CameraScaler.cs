using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ice {

  /// General 2D camera scaler for pixel perfect cameras and various other types of scaling
  /// strategies. Detailed description at:
  /// https://www.reddit.com/r/Unity2D/comments/4q0pxe/orthographic_camera_scaling_strategies_for/
  [ExecuteInEditMode]
  public class CameraScaler : MonoBehaviour {

    public enum ScaleMode {
      // Maintain a fixed ratio of world units to screen pixels, regardless of window size.
      FIXED_SCALE = 0,

      // Pixel-perfect scale to the given target viewport. Will always show at least the target
      // viewport height. Excess will be letterboxed if letterboxing is set.
      MIN_VIEWPORT = 1,
    }
    public ScaleMode mode;

    // TODO: Add letterboxing

    [SerializeField]
    [Tooltip("Outputs a pixel-perfect render texture to the given mesh's material.")]
    private MeshRenderer renderMesh;

    [Tooltip("Ratio of world units to screen pixels, this must be set.")]
    [SerializeField]
    private int pixelsPerUnit = 100;

    [Tooltip("Constant scale factor to apply, for FIXED_SCALE mode.")]
    [SerializeField]
    private int fixedScaleFactor = 1;

    [Tooltip("Desired viewport height in world units.  No effect in FIXED_SCALE mode.")]
    [SerializeField]
    private float targetViewportWorldHeight;

    private new Camera camera;
    private int screenWidth, screenHeight;

    private void Start() {
      // TODO: Cached.
      camera = GetComponent<Camera>();
    }

    private void Update() {
      if (screenHeight != Screen.height || screenWidth != Screen.width) {
        Apply();
      }
    }

    private void Apply() {
      screenHeight = Screen.height;
      screenWidth = Screen.width;
      float scaleFactor = 1.0f;
      float aspect = (float)screenWidth / screenHeight;
      if (mode == ScaleMode.FIXED_SCALE) {
        scaleFactor = fixedScaleFactor;
      } else {
        var minWorldPixels = targetViewportWorldHeight * pixelsPerUnit;
        scaleFactor = Mathf.Floor(screenHeight / minWorldPixels);
      }
      var ortho = screenHeight / 2.0f / (float)pixelsPerUnit / (float)scaleFactor;
      int actualWorldPixelHeight = (int)(ortho * 2 * pixelsPerUnit);
      int actualWorldPixelWidth = (int)(ortho * 2 * pixelsPerUnit * aspect);
      camera.orthographicSize = ortho;

      ReleaseRenderTexture();
      if (renderMesh != null) {
        CreatePixelRenderTexture(actualWorldPixelHeight, actualWorldPixelWidth);
      }
    }

    private void ReleaseRenderTexture() {
      if (camera.targetTexture != null) {
        camera.targetTexture.Release();
      }
      camera.targetTexture = null;
    }

    /// Creates a render texture sized for pixel-perfect scaling, and applies it to the
    /// mesh target.
    private void CreatePixelRenderTexture(int worldPixelHeight, int worldPixelWidth) {
      RenderTexture tex = new RenderTexture(worldPixelWidth, worldPixelHeight, 24);
      //tex.format = RenderTextureFormat
      tex.filterMode = FilterMode.Point;
      camera.targetTexture = tex;
      renderMesh.sharedMaterial.mainTexture = tex;
    }
  }

} // namespace Ice

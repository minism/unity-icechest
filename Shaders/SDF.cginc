/**
 * Signed distance function operations for shaders.
 */
			#include "UnityCG.cginc"

//// PRIMITIVES

// Torus
// t.x: Diameter
// t.y: Thickness
float sdTorus(float3 p, float2 t) {
  float2 q = float2(length(p.xz) - t.x, p.y);
  return length(q) - t.y;
}

/**
 * Signed distance function operations for shaders.
 *
 * Reference: https://www.iquilezles.org/www/articles/distfunctions/distfunctions.htm
 */

#include "UnityCG.cginc"

//// Primitives

float sdSphere(float3 p, float r) {
  return length(p) - r;
}

float sdBox(float3 p, float3 b) {
  float3 d = abs(p) - b;
  return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
}

float sdCube(float3 p, float l) {
  return sdBox(p, float3(l, l, l));
}

// Torus
// t.x: Diameter
// t.y: Thickness
float sdTorus(float3 p, float2 t) {
  float2 q = float2(length(p.xz) - t.x, p.y);
  return length(q) - t.y;
}


//// Distance ops
float opSub(float d1, float d2) {
  return max(-d1, d2);
}

float opUnion(float d1, float d2) {
  return min(d1, d2);
}

float opSmoothUnion(float d1, float d2, float k) {
  float h = max( k-abs(d1 - d2), 0.0 )/k;
  return min(d1, d2) - h*h*k*(1.0/4.0);
}




//// Domain ops

float3 mod(float3 x, float3 y) {
  return x - y * floor(x / y);
}

float3 opRep(float3 p, float3 c) {
  return mod(p + 0.5 * c, c) - 0.5 * c;
}

float3 opRepClamped(float3 p, float size, float3 instances) {
  return p - size * clamp(round(p / size), -instances, instances);
}

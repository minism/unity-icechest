float4 hash4( float2 p ) {
  return frac(sin(float4( 1.0+dot(p,float2(37.0,17.0)), 
                          2.0+dot(p,float2(11.0,47.0)),
                          3.0+dot(p,float2(41.0,29.0)),
                          4.0+dot(p,float2(23.0,31.0))))*103.0);
}

float4 sample_texture_repeated_1 (sampler2D tex, float2 uv)
{
  // "Technique 1" repetition from:
  // http://www.iquilezles.org/www/articles/texturerepetition/texturerepetition.htm
  float2 iuv = floor( uv );
  float2 fuv = frac( uv );

  // generate per-tile transform
  float4 ofa = hash4( iuv + float2(0,0) );
  float4 ofb = hash4( iuv + float2(1,0) );
  float4 ofc = hash4( iuv + float2(0,1) );
  float4 ofd = hash4( iuv + float2(1,1) );

  float2 dx = ddx(uv);
  float2 dy = ddy( uv );

  // transform per-tile uvs
  ofa.zw = sign( ofa.zw-0.5 );
  ofb.zw = sign( ofb.zw-0.5 );
  ofc.zw = sign( ofc.zw-0.5 );
  ofd.zw = sign( ofd.zw-0.5 );

  // uv's, and derivatives (for correct mipmapping)
  float2 uva = uv*ofa.zw + ofa.xy, ddxa = dx*ofa.zw, ddya = dy*ofa.zw;
  float2 uvb = uv*ofb.zw + ofb.xy, ddxb = dx*ofb.zw, ddyb = dy*ofb.zw;
  float2 uvc = uv*ofc.zw + ofc.xy, ddxc = dx*ofc.zw, ddyc = dy*ofc.zw;
  float2 uvd = uv*ofd.zw + ofd.xy, ddxd = dx*ofd.zw, ddyd = dy*ofd.zw;
  
  // fetch and blend
  float2 b = smoothstep( 0.25,0.75, fuv );

  return lerp( lerp( tex2Dgrad( tex, uva, ddxa, ddya ), 
                     tex2Dgrad( tex, uvb, ddxb, ddyb ), b.x ), 
               lerp( tex2Dgrad( tex, uvc, ddxc, ddyc ),
                     tex2Dgrad( tex, uvd, ddxd, ddyd ), b.x), b.y );
}
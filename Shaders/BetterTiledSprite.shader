// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// From http://www.iquilezles.org/www/articles/texturerepetition/texturerepetition.htm

Shader "Ice/Better Tiled Sprite"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)
    [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
  }

  SubShader
  {
    Tags
    { 
      "Queue"="Transparent" 
      "IgnoreProjector"="True" 
      "RenderType"="Transparent" 
      "PreviewType"="Plane"
      "CanUseSpriteAtlas"="True"
    }

    Cull Off
    Lighting Off
    ZWrite Off
    Blend One OneMinusSrcAlpha

    Pass
    {
    CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile _ PIXELSNAP_ON
      #include "UnityCG.cginc"
      
      struct appdata_t
      {
        float4 vertex   : POSITION;
        float4 color    : COLOR;
        float2 texcoord : TEXCOORD0;
      };

      struct v2f
      {
        float4 vertex   : SV_POSITION;
        fixed4 color    : COLOR;
        float2 texcoord  : TEXCOORD0;
      };
      
      fixed4 _Color;

      v2f vert(appdata_t IN)
      {
        v2f OUT;
        OUT.vertex = UnityObjectToClipPos(IN.vertex);
        OUT.texcoord = IN.texcoord;
        OUT.color = IN.color * _Color;
        #ifdef PIXELSNAP_ON
        OUT.vertex = UnityPixelSnap (OUT.vertex);
        #endif

        return OUT;
      }

      sampler2D _MainTex;
      sampler2D _AlphaTex;
      float _AlphaSplitEnabled;

      float4 hash4( float2 p ) {
        return frac(sin(float4( 1.0+dot(p,float2(37.0,17.0)), 
                                2.0+dot(p,float2(11.0,47.0)),
                                3.0+dot(p,float2(41.0,29.0)),
                                4.0+dot(p,float2(23.0,31.0))))*103.0);
      }

      fixed4 SampleSpriteTexture (float2 uv)
      {
        // "Technique 1" repetition from:
        // http://www.iquilezles.org/www/articles/texturerepetition/texturerepetition.htm

        fixed4 color = tex2D (_MainTex, uv);
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
    
        return lerp( lerp( tex2Dgrad( _MainTex, uva, ddxa, ddya ), 
                         tex2Dgrad( _MainTex, uvb, ddxb, ddyb ), b.x ), 
                    lerp( tex2Dgrad( _MainTex, uvc, ddxc, ddyc ),
                         tex2Dgrad( _MainTex, uvd, ddxd, ddyd ), b.x), b.y );
      }

      fixed4 frag(v2f IN) : SV_Target
      {
        fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
        c.rgb *= c.a;
        return c;
      }
    ENDCG
    }
  }
}

Shader "CoinGarden/WhiteGround" {
 Properties { _Color ("Color", Color) = (0.94,0.95,0.94,1) }
 SubShader {
 Tags { "RenderType"="Opaque" }
 Pass {
 Tags { "LightMode"="ForwardBase" }
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fwdbase
 #include "UnityCG.cginc"
 #include "AutoLight.cginc"
 struct v2f { float4 pos:SV_POSITION; SHADOW_COORDS(0) };
 fixed4 _Color;
 v2f vert(appdata_base v) { v2f o; o.pos=UnityObjectToClipPos(v.vertex); TRANSFER_SHADOW(o); return o; }
 fixed4 frag(v2f i):SV_Target { fixed shade=SHADOW_ATTENUATION(i); return fixed4(_Color.rgb*lerp(.64,1,shade),1); }
 ENDCG
 }
 }
 Fallback "Diffuse"
}

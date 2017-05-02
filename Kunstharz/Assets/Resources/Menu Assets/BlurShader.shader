// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlurShader"
	{
     Properties
     {
         _MainTex ("Texture", 2D) = "white" { }     
     }
 
     SubShader
     {
         // Horizontal blur pass
         Pass
         {
             Blend SrcAlpha OneMinusSrcAlpha 
             Name "HorizontalBlur"
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag            
             #include "UnityCG.cginc"
             
             sampler2D _MainTex;
             
             struct v2f
             {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
             };
             
             float4 _MainTex_ST;
             
             v2f vert (appdata_base v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                 return o;
             }
             
             half4 frag (v2f i) : COLOR
             {
                 float blurAmount = 0.0075;
 
                 half4 sum = half4(0.0, 0.0, 0.0, 0.0);
 
                 sum += tex2D(_MainTex, float2(i.uv.x - 5.0 * blurAmount, i.uv.y)) * 0.025;
                 sum += tex2D(_MainTex, float2(i.uv.x - 4.0 * blurAmount, i.uv.y)) * 0.05;
                 sum += tex2D(_MainTex, float2(i.uv.x - 3.0 * blurAmount, i.uv.y)) * 0.09;
                 sum += tex2D(_MainTex, float2(i.uv.x - 2.0 * blurAmount, i.uv.y)) * 0.12;
                 sum += tex2D(_MainTex, float2(i.uv.x - blurAmount, i.uv.y)) * 0.15;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * 0.16;
                 sum += tex2D(_MainTex, float2(i.uv.x + blurAmount, i.uv.y)) * 0.15;
                 sum += tex2D(_MainTex, float2(i.uv.x + 2.0 * blurAmount, i.uv.y)) * 0.12;
                 sum += tex2D(_MainTex, float2(i.uv.x + 3.0 * blurAmount, i.uv.y)) * 0.09;
                 sum += tex2D(_MainTex, float2(i.uv.x + 4.0 * blurAmount, i.uv.y)) * 0.05;
                 sum += tex2D(_MainTex, float2(i.uv.x + 5.0 * blurAmount, i.uv.y)) * 0.025;
 
                 sum.r = 1.0;
                 sum.g = 1.0;
                 sum.b = 0.0;
 
                 return sum;
             }
             ENDCG
         }
         
         GrabPass { }
 
         // Vertical blur pass
         Pass
         {
             Blend SrcAlpha OneMinusSrcAlpha
             Name "VerticalBlur"
                         
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag            
             #include "UnityCG.cginc"
 
             sampler2D _GrabTexture : register(s0);
 
             struct v2f 
             {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
             };
 
             float4 _GrabTexture_ST;
 
             v2f vert (appdata_base v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 o.uv = TRANSFORM_TEX(v.texcoord, _GrabTexture);
                 return o;
             }
 
             half4 frag (v2f i) : COLOR
             {
                 float blurAmount = 0.0075;
 
                 half4 sum = half4(0.0, 0.0, 0.0, 0.0);
 
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 5.0 * blurAmount)) * 0.025;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 4.0 * blurAmount)) * 0.05;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 3.0 * blurAmount)) * 0.09;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - 2.0 * blurAmount)) * 0.12;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y - blurAmount)) * 0.15;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y)) * 0.16;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + blurAmount)) * 0.15;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 2.0 * blurAmount)) * 0.12;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 3.0 * blurAmount)) * 0.09;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 4.0 * blurAmount)) * 0.05;
                 sum += tex2D(_GrabTexture, float2(i.uv.x, i.uv.y + 5.0 * blurAmount)) * 0.025;
 
                 sum.r = 1.0;
                 sum.g = 1.0;
                 sum.b = 0.0;
 
                 return sum;
             }
             ENDCG
         }
 
         // Original image pass
         Pass
         {
             ZWrite Off
              Blend SrcAlpha OneMinusSrcAlpha
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #include "UnityCG.cginc"
             sampler2D _MainTex;
             struct v2f
             {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
                 float2 depth : TEXCOORD1;
             };
 
             float4 _MainTex_ST;
             v2f vert (appdata_base v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 UNITY_TRANSFER_DEPTH(o.depth);
                 o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                 return o;
             }
 
             half4 frag (v2f i) : COLOR
             {
                 half4 originalPixel = half4(0.0, 0.0, 0.0, 0.0);
                 originalPixel = tex2D(_MainTex, float2(i.uv.x, i.uv.y));
 
                 return originalPixel;
             }
             ENDCG
         }
     }
 
     Fallback "VertexLit"
 }
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Shockwave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Origin("Origin", Vector) = (0,0,0)
		_Progress("Progress", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 ray : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				//o.vertex = UnityObjectToClipPos(v.vertex);
				//o.screenPosition = ComputeScreenPos(o.vertex);
				//UNITY_TRANSFER_DEPTH(o.depth);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = ComputeScreenPos(o.vertex);

				return o;
			}
			
			sampler2D _MainTex;
			uniform sampler2D _CameraDepthTexture;
			half _Progress;
			float4 _Origin;
			float4x4 _ViewProjectInverse;

			fixed4 frag (v2f i) : SV_Target
			{
				// from depth to world
				const float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
				const float2 p13_31 = float2(unity_CameraProjection._13, unity_CameraProjection._23);
				const float isOrtho = unity_OrthoParams.w;
				const float near = _ProjectionParams.y;
				const float far = _ProjectionParams.z;

				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				#if defined(UNITY_REVERSED_Z)
							d = 1 - d;
				#endif
				float zOrtho = lerp(near, far, d);
				float zPers = near * far / lerp(far, near, d);
				float vz = lerp(zPers, zOrtho, isOrtho);

				float3 vpos = float3((i.uv * 2 - 1 - p13_31) / p11_22 * lerp(vz, 1, isOrtho), -vz);
				float4 wpos = mul(_ViewProjectInverse, float4(vpos, 1));

				// shockwave
				float dist = distance(wpos, _Origin);

				fixed4 c = tex2D(_MainTex, i.uv);

				float progress = _Progress * 10;

				if (dist < _Progress * 8 && dist > _Progress*_Progress * 20) {
					c = fixed4(1.0,1.0,1.0,1.0);
				}
				

				return c;
			}
			ENDCG
		}
	}
}

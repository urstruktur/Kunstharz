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
			float _Actionness;

			float3 Hue(float H)
			{
			    float R = abs(H * 6 - 3) - 1;
			    float G = 2 - abs(H * 6 - 2);
			    float B = 2 - abs(H * 6 - 4);
			    return saturate(float3(R,G,B));
			}

			float3 HSVtoRGB(in float3 HSV)
			{
			    return ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
			}

			float3 RGBtoHSV(in float3 RGB)
			{
			    float3 HSV = 0;
			    HSV.z = max(RGB.r, max(RGB.g, RGB.b));
			    float M = min(RGB.r, min(RGB.g, RGB.b));
			    float C = HSV.z - M;
			    if (C != 0)
			    {
			        HSV.y = C / HSV.z;
			        float3 Delta = (HSV.z - RGB) / C;
			        Delta.rgb -= Delta.brg;
			        Delta.rg += float2(2,4);
			        if (RGB.r >= HSV.z)
			            HSV.x = Delta.b;
			        else if (RGB.g >= HSV.z)
			            HSV.x = Delta.r;
			        else
			            HSV.x = Delta.g;
			        HSV.x = frac(HSV.x / 6);
			    }
			    return HSV;
			}


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

				float3 hsv = RGBtoHSV(c.xyz);
				hsv.y = lerp(hsv.y * 0.75, hsv.y * 1.0, _Actionness);
				c = float4(HSVtoRGB(hsv), 1.0);

				return c;
			}
			ENDCG
		}
	}
}

// Colorful FX - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

Shader "Hidden/Colorful/Noise"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Params ("Seed (X) Strength (Y)", Vector) = (0, 0, 0, 0)
	}

	CGINCLUDE

		#include "UnityCG.cginc"
		#include "./Colorful.cginc"

		sampler2D _MainTex;
		half2 _Params;

		half4 frag_mono(v2f_img i) : SV_Target
		{
			half4 color = tex2D(_MainTex, i.uv);
			half n = simpleNoise(i.uv + _Params.x) * 2.0;
			return lerp(color, color * n, _Params.y);
		}

		half4 frag_colored(v2f_img i) : SV_Target
		{
			half4 color = tex2D(_MainTex, i.uv);
			half nr = simpleNoise(i.uv + _Params.x) * 2.0;
			half ng = simpleNoise(i.uv + _Params.x * 0.5) * 2.0;
			half nb = simpleNoise(i.uv + _Params.x * 0.25) * 2.0;
			return lerp(color, color * half4(nr, ng, nb, 1.0), _Params.y);
		}

	ENDCG

	SubShader
	{
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		// (0) Monochrome
		Pass
		{			
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_mono
				#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG
		}

		// (1) Colored
		Pass
		{			
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag_colored
				#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG
		}
	}

	FallBack off
}

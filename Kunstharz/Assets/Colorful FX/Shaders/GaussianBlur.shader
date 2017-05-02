﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Colorful FX - Unity Asset
// Copyright (c) 2015 - Thomas Hourdel
// http://www.thomashourdel.com

Shader "Hidden/Colorful/Gaussian Blur"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Direction ("Direction (XY)", Vector) = (0, 0, 0, 0)
	}

	CGINCLUDE
	ENDCG

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				half2 _Direction;

				struct fInput
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 uv1 : TEXCOORD1;
					float4 uv2 : TEXCOORD2;
				};

				fInput vert(appdata_img v)
				{
					fInput o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;
					float2 d1 = 1.3846153846 * _Direction;
					float2 d2 = 3.2307692308 * _Direction;
					o.uv1 = float4(o.uv + d1, o.uv - d1);
					o.uv2 = float4(o.uv + d2, o.uv - d2);
					return o;
				}

				half4 frag(fInput i) : SV_Target
				{
					half4 oc = tex2D(_MainTex, i.uv);
					half3 c = oc.rgb * 0.2270270270;
					c += tex2D(_MainTex, i.uv1.xy).rgb * 0.3162162162;
					c += tex2D(_MainTex, i.uv1.zw).rgb * 0.3162162162;
					c += tex2D(_MainTex, i.uv2.xy).rgb * 0.0702702703;
					c += tex2D(_MainTex, i.uv2.zw).rgb * 0.0702702703;
					return half4(c, oc.a);
				}

			ENDCG
		}
	}

	FallBack off
}

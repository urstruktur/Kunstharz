// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Superformula"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Intensity("Intensity", Float) = 0
		_M("M", Float) = 0
		_N1("N1", Float) = 0
		_N2("N2", Float) = 0
		_N3("N3", Float) = 0
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
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	float _M;
	float _N1;
	float _N2;
	float _N3;
	float _Intensity;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 c = tex2D(_MainTex, i.uv);

		float a = 1.0;
		float b = 1.0;

		float2 uv = i.uv;

		// translation
		float2 sf = uv - 0.5;

		// r of the current point
		float pr = sqrt(sf.x*sf.x + sf.y*sf.y);

		// Angle (Phi) of the current point
		float f = atan(sf.y / sf.x);

		// The formula itself. Division by 10.0 added to scale the result down a bit.
		float r = pow((pow(abs(cos(f*_M / 4.0) / a), _N2) + pow(abs(sin(f*_M / 4.0) / b), _N3)), -(1.0 / _N1)) /10;

		// Output with coloring by relative radius
		//c = (pr <= r) ?
		 //float4(r*3, 0.0, r*3, 1.0) :
		// float4(r * 3, 0.0, r*2, 1.0) * c;
		c += float4(r / pr, r / pr, r / pr, 1.0) * _Intensity;

		return c;
	}
		ENDCG
	}
	}
}

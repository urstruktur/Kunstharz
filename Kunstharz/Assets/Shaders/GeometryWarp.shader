// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Kunstharz/GeometryWarp" {
	Properties{
		_Tess("Tessellation", Range(1,32)) = 32
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DispTex("Disp Texture", 2D) = "gray" {}
		_NormalMap("Normalmap", 2D) = "bump" {}
		_Displacement("Displacement", Range(0, 5.0)) = 1.0
		_Origin("Origin", Vector) = (0,0,0)
		_Color("Color", color) = (1,1,1,0)
		_SpecColor("Spec color", color) = (0.5,0.5,0.5,0.5)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 300

		CGPROGRAM
		#pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
		#pragma target 4.6
		#include "Tessellation.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
	};

	float _Tess;

	sampler2D _DispTex;
	float _Displacement;
	float3 _Origin;

	// Displace geometry based on vertex distance to _Origin

	float getDisplacementFactor(float3 v) {
		return _Displacement / pow(distance(v, _Origin), 1.2);
	}

	void disp(inout appdata v)
	{
		float3 d = getDisplacementFactor(v.vertex)*(v.vertex - _Origin);
		v.vertex.xyz += d;
	}

	struct Input {
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	sampler2D _NormalMap;
	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Specular = 0.2;
		o.Gloss = 1.0;
		o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
	}

	float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
	{
		float dist = distance(vertex, _Origin);
		//float dist = 1/getDisplacementFactor(vertex)*2;
		float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
		return f;
	}

	// Distance based tessellation:
	// Tessellation level is "tess" before "minDist" from camera, and linearly decreases to 1
	// up to "maxDist" from camera.
	float4 DistanceBasedTess(float4 v0, float4 v1, float4 v2, float minDist, float maxDist, float tess)
	{
		float3 f;

		f.x = CalcDistanceTessFactor(v0, minDist, maxDist, tess);
		f.y = CalcDistanceTessFactor(v1, minDist, maxDist, tess);
		f.z = CalcDistanceTessFactor(v2, minDist, maxDist, tess);

		return UnityCalcTriEdgeTessFactors(f);
	}

	float4 tessDistance(appdata v0, appdata v1, appdata v2) {
		float minDist = 2.0;
		float maxDist = 5.0;
		return DistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
	}

	ENDCG
	}
		FallBack "Diffuse"
}
﻿Shader "WorldUV/BumpedSpecNocolor"
{
	Properties
	{
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Shininess ("Shininess", Range (0.01, 1.5)) = 0.078125
		_MainTex ("Base", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_ScaleX ("Scale X", float) = 1.0
		_ScaleY ("Scale Y", float) = 1.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		//LOD 400

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf VAO

		half _Shininess;

		#include "../lightingvao.cginc"

		struct Input
		{
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;

		half _ScaleX;
		half _ScaleY;

		void surf (Input IN, inout SurfaceOutput o)
		{
			float3 correctWorldNormal = WorldNormalVector(IN, float3(0.0, 0.0, 1.0));
			float2 uv = IN.worldPos.xz;

			if (abs(correctWorldNormal.x) > 0.5)
				uv = IN.worldPos.zy;
			else if (abs(correctWorldNormal.z) > 0.5)
				uv = IN.worldPos.xy;

			uv.x *= _ScaleX;
			uv.y *= _ScaleY;

			o.Albedo = tex2D(_MainTex, uv).rgb;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, uv));
		}

		ENDCG
	}

	FallBack "WorldUV/BumpedNocolor"
}
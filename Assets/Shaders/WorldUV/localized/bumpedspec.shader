Shader "WorldUV/Localized/BumpedSpec"
{
	Properties
	{
		_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
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

		#include "../../lightingvao.cginc"

		struct Input
		{
			float4 color : COLOR;
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;

		half _ScaleX;
		half _ScaleY;

		void surf (Input IN, inout SurfaceOutput o)
		{
			float3 correctWorldNormal = WorldNormalVector(IN, float3(0.0, 0.0, 1.0));
			float3 pos = IN.worldPos - mul(_Object2World, float4(0.0, 0.0, 0.0, 1.0));
			float2 uv = pos.xz;

			if (abs(correctWorldNormal.x) > 0.5)
				uv = pos.zy;
			else if (abs(correctWorldNormal.z) > 0.5)
				uv = pos.xy;

			uv.x *= _ScaleX;
			uv.y *= _ScaleY;

			o.Albedo = tex2D(_MainTex, uv).rgb * IN.color.rgb * _Color.rgb;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, uv));
		}

		ENDCG
	}

	FallBack "WorldUV/Localized/Bumped"
}
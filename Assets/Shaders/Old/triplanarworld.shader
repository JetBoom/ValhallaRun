// This shader is extremely useful for world building and terrain.
// There is no need to assign texture UV coordinates. The shader aligns the textures to a fixed world grid.

Shader "Tri-Planar World" {
  Properties {
		_Side("Side", 2D) = "white" {}
		_Top("Top", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
		_SideScale("Side Scale", Float) = 2
		_TopScale("Top Scale", Float) = 2
		_BottomScale ("Bottom Scale", Float) = 2
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"IgnoreProjector"="False"
			"RenderType"="Opaque"
		}
 
		Cull Back
		ZWrite On
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma exclude_renderers flash
		#pragma target 3.0
 
		sampler2D _Side, _Top, _Bottom;
		float _SideScale, _TopScale, _BottomScale;
		
		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};
			
		void surf (Input IN, inout SurfaceOutput o) {
			float3 worldNormal = WorldNormalVector(IN, float3(0, 0, 1)); // Can't use IN.worldNormal if changing o.Normal
			float3 projNormal = saturate(pow(worldNormal * 1.4, 4));
			float3 x, y, z; // Albedo
			
			// SIDE X
			x = tex2D(_Side, frac(IN.worldPos.zy * _SideScale)) * abs(worldNormal.x);
			
			// TOP / BOTTOM
			if (IN.worldNormal.y > 0)
				y = tex2D(_Top, frac(IN.worldPos.zx * _TopScale)) * abs(worldNormal.y);
			else
				y = tex2D(_Bottom, frac(IN.worldPos.zx * _BottomScale)) * abs(worldNormal.y);
			
			// SIDE Z	
			z = tex2D(_Side, frac(IN.worldPos.xy * _SideScale)) * abs(worldNormal.z);
			
			o.Albedo = z;
			o.Albedo = lerp(o.Albedo, x, projNormal.x);
			o.Albedo = lerp(o.Albedo, y, projNormal.y);
		}
		ENDCG
	}
	Fallback "Diffuse"
}
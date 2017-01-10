// This shader is extremely useful for world building and terrain.
// There is no need to assign texture UV coordinates. The shader aligns the textures to a fixed world grid.
// Bump mapped.

Shader "Tri-Planar World Bumped" {
  Properties {
		_Side("Side", 2D) = "white" {}
		_Top("Top", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
		_BumpMapSide("Side Normal Map", 2D) = "bump" {}
		_BumpMapTop("Top Normal Map", 2D) = "bump" {}
		_BumpMapBottom("Bottom Normal Map", 2D) = "bump" {}
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
		sampler2D _BumpMapSide, _BumpMapTop, _BumpMapBottom;
		float _SideScale, _TopScale, _BottomScale;
		
		struct Input {
			float3 worldPos;
			float3 worldNormal; INTERNAL_DATA
		};
			
		void surf (Input IN, inout SurfaceOutput o) {
			float3 worldNormal = WorldNormalVector(IN, float3(0, 0, 1)); // Can't use IN.worldNormal if changing o.Normal
			float3 projNormal = saturate(pow(worldNormal * 1.4, 4));
			float3 x, y, z; // Albedo
			half3 dX, dY, dZ; // Normal
			
			// SIDE X
			x = tex2D(_Side, frac(IN.worldPos.zy * _SideScale)) * abs(worldNormal.x);
			dX = UnpackNormal(tex2D(_BumpMapSide, frac(IN.worldPos.zy * _SideScale)) * abs(worldNormal.x));
			
			// TOP / BOTTOM
			if (IN.worldNormal.y > 0)
			{
				y = tex2D(_Top, frac(IN.worldPos.zx * _TopScale)) * abs(worldNormal.y);
				dY = UnpackNormal(tex2D(_BumpMapTop, frac(IN.worldPos.zx * _TopScale)) * abs(worldNormal.y));
			}
			else
			{
				y = tex2D(_Bottom, frac(IN.worldPos.zx * _BottomScale)) * abs(worldNormal.y);
				dY = UnpackNormal(tex2D(_BumpMapBottom, frac(IN.worldPos.zx * _BottomScale)) * abs(worldNormal.y));
			}
			
			// SIDE Z	
			z = tex2D(_Side, frac(IN.worldPos.xy * _SideScale)) * abs(worldNormal.z);
			dZ = UnpackNormal(tex2D(_BumpMapSide, frac(IN.worldPos.xy * _SideScale)) * abs(worldNormal.z));
			
			o.Albedo = z;
			o.Albedo = lerp(o.Albedo, x, projNormal.x);
			o.Albedo = lerp(o.Albedo, y, projNormal.y);

			o.Normal = dZ;
			o.Normal = lerp(o.Normal, dX, projNormal.x);
			o.Normal = lerp(o.Normal, dY, projNormal.y);
		} 
		ENDCG
	}
	Fallback "Diffuse"
}
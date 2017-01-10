Shader "Cloud Surface" {
  Properties {
		_Tex("Tex", 2D) = "white" {}
		_TexScale("Tex Scale", Float) = 0.25
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"IgnoreProjector"="False"
			"RenderType"="Transparent"
		}
 
		Cull Back
		ZWrite On
		
		CGPROGRAM
		#pragma surface surf NoLighting alpha
		#pragma exclude_renderers flash
 
		sampler2D _Tex;
		float _TexScale;
		
		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};
			
		void surf (Input IN, inout SurfaceOutput o) {
			float3 projNormal = saturate(pow(IN.worldNormal * 1.4, 4));
			//float4 ftime = time;
			
			// SIDE X
			float3 x = tex2D(_Tex, frac(IN.worldPos.zy * _TexScale + _SinTime * 2)) * abs(IN.worldNormal.x);
			
			// TOP / BOTTOM
			float3 y = tex2D(_Tex, frac(IN.worldPos.zx * _TexScale + _SinTime * 6)) * abs(IN.worldNormal.y);
			
			// SIDE Z	
			float3 z = tex2D(_Tex, frac(IN.worldPos.xy * _TexScale)) * abs(IN.worldNormal.z);
			
			o.Albedo = z;
			o.Albedo = lerp(o.Albedo, x, projNormal.x);
			o.Albedo = lerp(o.Albedo, y, projNormal.y);
			o.Emission = 0.025;
			o.Alpha = 0.95 + sin(_Time * 32) * 0.05;
		} 

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;

			c.rgb = s.Albedo * 0.2; 
			c.a = s.Alpha;

			return c;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
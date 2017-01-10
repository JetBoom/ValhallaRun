inline fixed4 LightingVAO( SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten )
{
	half3 h = normalize( lightDir + viewDir );
	fixed diff = max( 0.0, dot( s.Normal, lightDir ) );

	float nh = max( 0.0, dot ( s.Normal, h ) );
	float spec = pow( nh, s.Specular * 128.0 );

	half3 c = ( s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec ) * atten;

	return fixed4( c, 1.0 );
}
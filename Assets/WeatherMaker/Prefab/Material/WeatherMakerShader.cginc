//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

fixed4 _TintColor;
fixed3 _EmissiveColor;
float _DirectionalLightMultiplier;
float _PointSpotLightMultiplier;
float _AmbientLightMultiplier;

sampler2D _MainTex;
float4 _MainTex_ST;

#if defined(SOFTPARTICLES_ON)

float _InvFade;
sampler2D _CameraDepthTexture;

#endif

inline fixed LerpFade(float4 lifeTime, float t)
{
	// the vertex will fade in, stay at full color, then fade out
	// x = creation time seconds
	// y = fade time in seconds
	// z = life time seconds

	// debug
	// return 1;

	float peakFadeIn = lifeTime.x + lifeTime.y;
	if (t < peakFadeIn)
	{
		return lerp(0, 1, max(0, ((t - lifeTime.x) / (peakFadeIn - lifeTime.x))));
	}
	float endLifetime = (lifeTime.x + lifeTime.z);
	float startFadeOut = endLifetime - lifeTime.y;

	// will be negative until fade out time (startFadeOut) is passed which will keep it at full color
	return lerp(1, 0, max(0, ((t - startFadeOut) / (endLifetime - startFadeOut))));
}

fixed3 ApplyLight(float4 lightPos, float4 lightDir, fixed3 lightColor, half4 lightAtten, float3 viewPos)
{
	float3 toLight = (lightPos.xyz - (viewPos * lightPos.w));
	float lengthSq = dot(toLight, toLight);
	float atten = 1.0 / (1.0 + (lengthSq * lightAtten.z));
	toLight = normalize(toLight);

#if defined(ORTHOGRAPHIC_MODE)

	float3 normal = fixed3(0, 0, 1);
	float diff = max(lightPos.w, dot(normal, toLight));
	return lightColor.rgb * (diff * atten);

#else

	// calculate modifier for non-directional light, will be 0,0,0 for directional lights
	float modifierNonDirectionalLight = lightPos.w;
	float3 normal = toLight * modifierNonDirectionalLight;

	// spot light calculation - should stay as 1 for point lights
	float rho = max(0, dot(toLight, lightDir.xyz));
	float spotAtt = max(0, (rho - lightAtten.x) * lightAtten.y);

	// add normal for directional lights of toLight - will be 0,0,0 for non-directional lights
	// we create a custom modifier for directional lights that works better than a simple normal calculation
	float4 lightPosWorld = mul(UNITY_MATRIX_T_MV, lightPos);
	float modifierDirectionalLight = 1.0 - modifierNonDirectionalLight;
	normal += (toLight * modifierDirectionalLight);

	// apply spot modifier last
	modifierNonDirectionalLight *= spotAtt;

	atten *= max(0, dot(normal, toLight));

	return lightColor.rgb *
		((atten * modifierNonDirectionalLight) +
		(clamp((lightPosWorld.y * 2) + 1.5, 0, 1) * modifierDirectionalLight));

#endif

}

inline fixed4 CalculateVertexColor(float3 viewPos)
{
	fixed3 vertexColor = UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientLightMultiplier;
	vertexColor += ApplyLight(unity_LightPosition[0], unity_SpotDirection[0], unity_LightColor[0], unity_LightAtten[0], viewPos);
	vertexColor += ApplyLight(unity_LightPosition[1], unity_SpotDirection[1], unity_LightColor[1], unity_LightAtten[1], viewPos);
	vertexColor += ApplyLight(unity_LightPosition[2], unity_SpotDirection[2], unity_LightColor[2], unity_LightAtten[2], viewPos);
	vertexColor += ApplyLight(unity_LightPosition[3], unity_SpotDirection[3], unity_LightColor[3], unity_LightAtten[3], viewPos);
	//vertexColor = clamp(vertexColor, fixed3(0, 0, 0), fixed3(1, 1, 1));

	return fixed4(vertexColor, 1);// max(vertexColor.r, max(vertexColor.g, vertexColor.b)));
}

inline float3 RotateVertexLocalQuaternion(float3 position, float3 axis, float angle)
{
	float half_angle = angle * 0.5;
	float _sin, _cos;
	sincos(half_angle, _sin, _cos);
	float4 q = float4(axis.xyz * _sin, _cos);
	return position + (2.0 * cross(cross(position, q.xyz) + (q.w * position), q.xyz));
}
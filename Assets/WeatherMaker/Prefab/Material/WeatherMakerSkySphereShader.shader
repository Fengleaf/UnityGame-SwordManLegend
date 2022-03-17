// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

Shader "WeatherMaker/WeatherMakerSkySphereShader"
{
	Properties
	{
		_MainTex ("Day Texture", 2D) = "blue" {}
		_NightTex("Night Texture", 2D) = "black" {}
		_SunTex("Sun Texture", 2D) = "yellow" {}
		_Exposure ("Day Multiplier", Range(0, 3)) = 1
		_NightMultiplier ("Night Multiplier", Range(0, 3)) = 0
		_NightVisibilityThreshold ("Night Visibility Threshold", Range(0, 1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Front

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uvMain : TEXCOORD0;
				float2 uvNight : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NightTex;
			float4 _NightTex_ST;
			sampler2D _SunTex;
			float4 _SunTex_ST;
			fixed _DayMultiplier;
			fixed _NightMultiplier;
			fixed _NightVisibilityThreshold;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uvMain = TRANSFORM_TEX(v.uv, _MainTex);
				o.uvNight = TRANSFORM_TEX(v.uv, _NightTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// almost works but leaves seams...
				//const fixed kOneOverPi = 1.0 / 3.14159;
				//fixed u = 0.5 - (0.5 * atan2(i.normal.x, -i.normal.z) * kOneOverPi);
				//fixed v = 1.0 - (acos(i.normal.y) * kOneOverPi);
				//fixed4 col = tex2D(_MainTex, fixed2(u, v));

				fixed4 dayColor = (tex2D(_MainTex, i.uvMain) * _DayMultiplier);
				fixed4 nightColor = (tex2D(_NightTex, i.uvNight) * _NightMultiplier);
				nightColor *= ceil(max(nightColor.r, max(nightColor.g, nightColor.b)) - _NightVisibilityThreshold);

				return (dayColor + nightColor);
			}

			ENDCG
		}
	}
}

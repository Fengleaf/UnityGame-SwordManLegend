Shader "Unlit/GradientColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_StartRed("StartRed", Range(0, 1)) = 0
		_StartGreen("StartGreen", Range(0, 1)) = 0
		_StartBlue("StartBlue", Range(0, 1)) = 0
		_EndRed("EndRed", Range(0, 1)) = 0
		_EndGreen("EndGreen", Range(0, 1)) = 0
		_EndBlue("EndBlue", Range(0, 1)) = 0
		_ColorOffset("_ColorOffset", Vector) = (0, 0.25, 0, 0)
		//MASK SUPPORT ADD
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        LOD 100

			//MASK SUPPORT ADD
		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}
		ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			float _StartRed;
			float _StartGreen;
			float _StartBlue;
			float _EndRed;
			float _EndGreen;
			float _EndBlue;
            float4 _MainTex_ST;
			vector _ColorOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float phase = _Time * 20;
				float red = clamp(i.uv.x + _ColorOffset.x, _StartRed, _EndRed);        // red: between startColor ~ 1
				float green = clamp(i.uv.x + _ColorOffset.y, _StartGreen, _EndGreen);        // green: between startColor ~ 1
				float blue = clamp(i.uv.x + _ColorOffset.z, _StartBlue, _EndBlue);        // green: between startColor ~ 1

				float4 fragColor = float4(red, green, blue, 0);

                UNITY_APPLY_FOG(i.fogCoord, fragColor);
                return fragColor;
            }
            ENDCG
        }
    }
}

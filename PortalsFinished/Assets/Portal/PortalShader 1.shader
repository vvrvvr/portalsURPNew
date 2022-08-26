Shader "Unlit/NewUnlitShader2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

	[Space(50)]
		[Header(Glitch Section)]
		[Space(10)]
		_GlitchStrength("Glitch Strength", Range(0,0.1)) = 0
		_GlitchDensity("Glitch Density", Float) = 0
		_GlitchPeriod("GlitchPeriod", Float) = 0
		_GlitchDirection("Glitch Direction", Vector) = (0,1,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull off

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
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _GlitchStrength;
			float _GlitchDensity;
			float _GlitchPeriod;
			float4 _GlitchDirection;
			float4 _HoloDirection;
			
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.y += sin(_GlitchPeriod * _Time.y + v.vertex.x * _GlitchDensity) * _GlitchStrength * _GlitchDirection.y;
				v.vertex.x += sin(_GlitchPeriod * _Time.y + v.vertex.y * _GlitchDensity) * _GlitchStrength * _GlitchDirection.x;
				v.vertex.z += sin(_GlitchPeriod * _Time.y + v.vertex.z * _GlitchDensity) * _GlitchStrength * _GlitchDirection.z;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv.xy / i.uv.w);
				return col;
			}
			ENDCG
		}
	}
}

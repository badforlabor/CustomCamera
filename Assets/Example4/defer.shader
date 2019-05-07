Shader "Unlit/defer"
{
	SubShader
	{
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.w = 1;

				//v.vertex.w = 1;
				//v.vertex.z = 1;
				//o.vertex = v.vertex;// mul(UNITY_MATRIX_P, v.vertex),
				o.uv = v.uv;
				return o;
			}
			
			float3 lightsPosition[32];
			float3 lightsColor[32];
			float lightsLinear[32];
			float lightsQuadratic[32];
			float3 _ViewPos;
			int _LightCount;

			Texture2D _GBuffer0; SamplerState sampler_GBuffer0;
			Texture2D _GBuffer1; SamplerState sampler_GBuffer1;
			Texture2D _GBuffer2; SamplerState sampler_GBuffer2;
			Texture2D _GBuffer3; SamplerState sampler_GBuffer3;

			fixed4 frag (v2f vfi) : SV_Target
			{
				float4 FragColor;

				float3 FragPos = _GBuffer0.Sample(sampler_GBuffer0, vfi.uv).rgb;
				float3 Normal = _GBuffer1.Sample(sampler_GBuffer1, vfi.uv).rgb;
				float3 Diffuse = _GBuffer2.Sample(sampler_GBuffer2, vfi.uv).rgb;
				float Specular = _GBuffer2.Sample(sampler_GBuffer2, vfi.uv).a;

				float3 lighting = Diffuse * 0.1;
				float3 viewDir = normalize(_ViewPos - FragPos);

				for (int i = 0; i < _LightCount; i++)
				{
					// diffuse
					float3 lightDir = normalize(lightsPosition[i] - FragPos);
					float3 diffuse = max(dot(Normal, lightDir), 0.0) * Diffuse * lightsColor[i];
					
					//lighting = diffuse;
					//break;
					
					// specular
					float3 halfwayDir = normalize(lightDir + viewDir);
					float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
					float3 specular = lightsColor[i] * spec * Specular;
					// attenuation
					float distance = length(lightsPosition[i] - FragPos);
					float attenuation = 1.0 / (1.0 + lightsLinear[i] * distance + lightsQuadratic[i] * distance * distance);
					diffuse *= attenuation;
					
					//lighting = diffuse;
					//break;

					specular *= attenuation;
					lighting += diffuse + specular;

				}
				FragColor = float4(lighting, 1.0);
				//FragColor = float4(FragPos, 1.0);
				//FragColor = float4(0, 1, 0, 1);
				//FragColor = float4(Diffuse, 1);
				//FragColor = float4(Normal, 1);
				return FragColor;
			}
			ENDCG
		}
	}
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/mesh"
{
	Properties
	{
		_DiffuseTex ("Diffuse", 2D) = "white" {}
		_SpecularTex("Specular", 2D) = "white" {}
	}    

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			// MRT shader
			struct FragmentOutput
			{
				half4 outGBuffer0 : SV_Target0;
				half4 outGBuffer1 : SV_Target1;
				half4 outGBuffer2 : SV_Target2;
			};

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 FragPos : TEXCOORD1;
				float3 Normal : TEXCOORD3;
				float4 vertex : SV_POSITION;
			};

			sampler2D _DiffuseTex;
			sampler2D _SpecularTex;
			float4x4 model;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.FragPos = worldPos;
				o.uv = v.uv;

				float4x4 normalMatrix = transpose(unity_WorldToObject);
				o.Normal = mul(normalMatrix, v.normal);

				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}
			
			void frag1 (v2f i, 
				out float4 outGBuffer0 : SV_Target0,
				out float4 outGBuffer1 : SV_Target1,
				out float4 outGBuffer2 : SV_Target2,
				out float4 outGBuffer3 : SV_Target3)
			{
				outGBuffer0 = float4(i.FragPos, 1);
				outGBuffer1 = float4(normalize(i.Normal), 1);
				outGBuffer2.rgb = tex2D(_DiffuseTex, i.uv).rgb;
				outGBuffer2.a = tex2D(_SpecularTex, i.uv).r;
			}

			FragmentOutput frag(v2f vfi)
			{
				FragmentOutput o;

				o.outGBuffer0 = float4(vfi.FragPos, 0);
				//o.outGBuffer0 = float4(1,0,0, 0);
				o.outGBuffer1 = float4(normalize(vfi.Normal), 1);
				o.outGBuffer2.rgb = tex2D(_DiffuseTex, vfi.uv).rgb;
				o.outGBuffer2.a = tex2D(_SpecularTex, vfi.uv).r;

				return o;
			}
			ENDCG
		}
	}
}

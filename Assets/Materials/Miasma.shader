Shader "Unlit/Miasma"
{
	Properties
	{
		_DetailScale ("Detail Scale", range(0, 10)) = 1
		_Detail ("Bumpiness", range(0, 1)) = 0.5
		_Color ("Color", Color) = (1,1,1,1)
		_OscillationSpeed ("Oscillation Speed", float) = 1 
		_OscillationAmount ("Oscillation Amount", range(0, 1)) = 0.5
		_MinA ("Min Alpha", float) = 0.1
		_MaxA ("Max Alpha", float) = 0.5
		_MaxDistance ("Max Distance", float) = 10
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100

		GrabPass {
			Name "BASE"
            Tags {}
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Simplex.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float dottednormal : TEXCOORD2;
				float3 campos : TEXCOORD3;
				float3 pos : TEXCOORD4;
				float4 screenUV : TEXCOORD5;
			};

			struct extra {
				float3 campos;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GrabTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.campos = UnityObjectToViewPos(v.vertex);
				o.pos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.dottednormal = dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal);
				o.screenUV = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			

			float _OscillationSpeed;
			float _OscillationAmount;

			float _MinA;
			float _MaxA;
			float _MaxDistance;

			float _Detail;
			float _DetailScale;

			float4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = float4(1,1,1,1);
				col.rgba *= _Color.rgba;

				float distanceRatio = - i.campos.z / _MaxDistance;
				distanceRatio = max(0, min(1, distanceRatio));
				col.a *= _MinA + (distanceRatio) * (_MaxA - _MinA);

				float st = (sin(_Time.y * _OscillationSpeed) / 2 + 0.5) * (_OscillationAmount)  + (1 - _OscillationAmount);
				col.a *= st;

				col.a *= max(0, min(1, abs(i.dottednormal)));


				float2 offset = float2(_SinTime.x * _OscillationSpeed / 10, _CosTime.x * _OscillationSpeed / 10);
				float2 offpos = float2(i.pos.x + cos(i.pos.y), i.pos.z + sin(i.pos.y));

				float3 pos = float3(i.pos.x * _DetailScale + _Time.y, i.pos.y * _DetailScale + _Time.x, i.pos.z * _DetailScale + _Time.z);

				fixed4 detail = snoise_grad(pos);
				detail *= _Detail;



				fixed4 grab = tex2Dproj(
					_GrabTexture, 
					i.screenUV + detail
				);

				fixed4 screen = tex2Dproj(
					_GrabTexture, 
					i.screenUV
				);

				//col.a = 1;
				float4 final = screen * (1 - col.a) + grab * col * col.a;
				final.a = 1;
				return final;
			}
			ENDCG
		}
	}
}

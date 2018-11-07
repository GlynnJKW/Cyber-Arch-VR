// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TriGeoScreenSizeShader"
{
	/*
	This shader renders the given vertices as circles with the given color.
	The point size is the radius of the circle given in pixel
	Implemented using geometry shader
	*/
	Properties{
		_PointSize("Point Size", Float) = 5
		_Shading("Shading Strength", Range(0, 1)) = 0.25
		_ScreenWidth("Screen Width", Int) = 0
		_ScreenHeight("Screen Height", Int) = 0
		[Toggle] _Circles("Circles", Int) = 0
	}

		SubShader
		{
			LOD 200

			Pass
			{
				Cull off

				CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag

				struct VertexInput
				{
					float4 position : POSITION;
					float4 color : COLOR;
				};

				struct VertexMiddle {
					float4 position : SV_POSITION;
					float4 color : COLOR;
				};

				struct VertexOutput
				{
					float4 position : SV_POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				float _PointSize;
				float _Shading;
				int _ScreenWidth;
				int _ScreenHeight;
				int _Circles;

				VertexMiddle vert(VertexInput v) {
					VertexMiddle o;
					o.position = UnityObjectToClipPos(v.position);
					o.color = v.color;
					return o;
				}

				[maxvertexcount(3)]
				void geom(point VertexMiddle input[1], inout TriangleStream<VertexOutput> outputStream) {
					float zfactor = input[0].position.z/input[0].position.w * 10 + 0.8;
					float xsize = _PointSize / (_ScreenWidth);
					xsize *= zfactor;
					float ysize = _PointSize / (_ScreenHeight);
					ysize *= zfactor;
					VertexOutput out1;
					out1.position = input[0].position;
					out1.color = input[0].color;
					out1.uv = float2(0.0f, 2.0f);
					out1.position.x -= out1.position.w * xsize;
					out1.position.y += out1.position.w * ysize;
					VertexOutput out2;
					out2.position = input[0].position;
					out2.color = input[0].color;
					out2.uv = float2(1.73205080757f, -1.0f);
					out2.position.x += out2.position.w * xsize;
					out2.position.y += out2.position.w * ysize;
					VertexOutput out3;
					out3.position = input[0].position;
					out3.color = input[0].color;
					out3.uv = float2(-1.73205080757f, -1.0f);
					out3.position.x += out3.position.w * xsize;
					out3.position.y -= out3.position.w * ysize;

					outputStream.Append(out1);
					outputStream.Append(out2);
					outputStream.Append(out3);
				}

				float4 frag(VertexOutput o) : COLOR{
					float rad = o.uv.x*o.uv.x + o.uv.y*o.uv.y;
					if (_Circles >= 0.5 && rad > 1) {
						discard;
					}
					return o.color * (_Shading * pow(1 - rad, 0.5) + (1 - _Shading));
				}

			ENDCG
		}
	}
}

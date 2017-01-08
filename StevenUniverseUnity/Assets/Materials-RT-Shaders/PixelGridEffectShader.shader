Shader "Hidden/Pixel Grid Effect" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_PixelsPerUnit("Pixels Per Unit", Float) = 16
		_TValue("TValue", Range(0,1) ) = .5
		_GridColor("Grid Color", Color) = (1,1,1,1)
		_XOffset("XOffset", Range(-20,20)) = 1
		_YOffset("YOffset", Range(-10,10)) = 1
	}

	SubShader
	{
		Pass
		{
			ZTest Always 
			Cull Off 
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed _PixelsPerUnit;
			fixed _TValue;
			fixed4 _GridColor;
			fixed _XOffset;
			fixed _YOffset;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 screenPos : TEXCOORD1;
			};

			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenPos : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//fixed t = _ScreenParams.y % 2;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// Screen position in 0
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}



			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed2 screenSize = _ScreenParams.xy;
				// Note if this was for perspective camera you should
				// divide by i.ScreenPos.w here

				/// UNCOMMENT HERE
				fixed2 pos = i.screenPos * screenSize;

				// Gives curtain effect from TValue for testing purposes
				if (i.uv.x < _TValue)
				{
					fixed2 ortho = unity_OrthoParams * 2;
					fixed2 scale = screenSize / ortho / _PixelsPerUnit;

					//fixed2 evenOffset = fixed2(_XOffset,0);
					//fixed2 oddOffset = fixed2(_YOffset,0);

					//fixed2 screen = floor(_ScreenParams / 2.0);
					//fixed2 evenOffset = fixed2(screen.x % _XOffset, 0);
					//fixed2 oddOffset = fixed2(screen.x % _YOffset,0);

					// 3X SCALE:
					//fixed2 evenOffset = _ScreenParams % -scale.y;
					//fixed2 oddOffset = (_ScreenParams - 1) % scale.y;

					// Divide our screen space by two so our mod by scale provides progressive values
					// given screen sizes which are always 2 apart
					fixed2 screen = floor(_ScreenParams / 2.0);
					fixed2 evenOffset = (screen) % scale.y;
					fixed2 oddOffset = (screen) % scale.y;
					
					//fixed screen = floor(_ScreenParams.y / 2.0);
					//fixed2 offset = fixed2(0,screen % _YOffset);
					//fixed2 evenOffset = offset;
					//fixed2 oddOffset = offset;


					pos -= lerp(evenOffset, oddOffset, _ScreenParams % 2 );

					pos = floor( pos  / scale);

					//fixed hpc = (screenSize / scale) * .5;
					//pos = round(i.screenPos * hpc) / hpc;
					//pos = round(pos);
					//pos = round( i.screenPos * (screenSize / 2)));
					//pos = floor(pos / _PixelsPerUnit);

					fixed t = (  pos.x + pos.y ) % 2;

					fixed3 gridColor = lerp(col.xyz, _GridColor.xyz, _GridColor.a);
					col.xyz = lerp(col.xyz, gridColor, t);
				}
				

				return col;
			}

			ENDCG
		}
	}

	Fallback off

}

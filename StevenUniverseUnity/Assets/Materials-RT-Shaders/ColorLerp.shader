Shader "Custom/ColorLerp"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Magnitude ( "Lerp Magnitude", Float ) = 1
		_LerpSpeed ( "Lerp Speed", Float ) = 1
		_Color ("COLOR", Color ) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM 
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct vertInput
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR0; // We will lerp from texture colors to mesh colors
				fixed2 uv : TEXCOORD0;
				//fixed3 lerpColor : NORMAL; // The color we'll lerp to
				//fixed2 lerpData : TEXCOORD0; // We'll write our lerp color alpha into uv0.x and our LerpSpeed into uv0.y
			};

			struct vertOutput
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR0;
				float2 uv : TEXCOORD0;
				//fixed4 primaryColor : COLOR0;
				//fixed3 lerpColor : COLOR1;
				//fixed2 lerpData : TEXCOORD0;
			};

			float _Magnitude;
			float4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _LerpSpeed;
			
			vertOutput vert(vertInput i )
			{
				vertOutput o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.vertex = UnityPixelSnap(o.vertex);

				o.color = i.color;
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				
				//o.primaryColor = i.primaryColor;
				//o.lerpColor = i.lerpColor;
				//o.lerpData = i.lerpData;
				return o;
			}
			
			float4 frag ( vertOutput o ) : SV_Target
			{
				fixed4 primary = tex2D(_MainTex, o.uv);
				fixed4 secondary = o.color;
				
				//fixed4 secondary = fixed4( o.lerpColor, o.lerpData.x );
				//fixed lerpSpeed = o.lerpData.y;
				
				//fixed t = 1 - cos( _Time.y * _LerpSpeed );
				//fixed t = _Time.y;
				// Gives a nice triangle wave
				//fixed t = 1 - abs( frac( _Time.y * _LerpSpeed ) - 1) * _Magnitude;
				fixed t = 1 - abs( frac( _Time.y * _LerpSpeed ) * 2 - 1 );
				//fixed t = (_SinTime.w * 5 + 1) / 2;
				//fixed4 col = lerp( primary, secondary, t );

				fixed4 col = lerp( primary, secondary, t * _Magnitude );

				return col * o.color;
			}
			ENDCG
		}
	}
}

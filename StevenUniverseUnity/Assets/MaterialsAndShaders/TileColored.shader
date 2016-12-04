Shader "Custom/TileColored"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		// Use the same method unity uses for pixel snap. In combination with the Pixel Snap camera asset
		// this SEEMS to fix all artifacts from camera scrolling
		[MaterialToggle] PixelSnap ("Pixel Snap", float) = 0
	}
	SubShader
	{
		Tags 
		{ 
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane" 
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			
			#include "UnityCG.cginc"

			struct vertInput
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			struct vertOutput
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			vertOutput vert(vertInput i )
			{
				vertOutput o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap(o.vertex);
				#endif
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				o.color = i.color;
				return o;
			}
			
			float4 frag ( vertOutput o ) : SV_Target
			{
				float4 color = tex2D(_MainTex, o.uv);
				color *= o.color;
				return color;
			}
			ENDCG
		}
	}
}

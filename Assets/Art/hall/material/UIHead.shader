Shader "miro/head" 
{
	Properties 
	{
		[PerRendererData] _MainTex ("Base (RGB)", 2D) = "white" {}
		_Radius ("_Radius",Range(0.01,0.75)) = 0.5
	}
	SubShader 
	{
		Blend SrcAlpha OneMinusSrcAlpha
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
			float _Radius;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D (_MainTex, i.uv);
				float _len = length(i.uv - float2(0.5,0.5));
				col.a = step(_len,_Radius);
				return col;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

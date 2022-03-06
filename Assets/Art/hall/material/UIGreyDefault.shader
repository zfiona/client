// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "miro/greyDefault" {
	Properties
    {
        [PerRendererData] _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
        _Stencil ("Stencil ID", int) = 1
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        
        Pass
        {
            Stencil{
				Ref [_Stencil]
				Comp Equal
			}
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag           
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
    
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }
                
            fixed4 frag (v2f IN) : COLOR
            {
                fixed4 col = tex2D(_MainTex, IN.texcoord);  
                fixed grey = dot(col.rgb, fixed3(0.299, 0.587, 0.114));  //0.299, 0.587, 0.114
                col.rgb = fixed3(grey, grey, grey);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
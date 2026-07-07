Shader "Unlit/2D_Sprite_Blur_Fixed"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Blur ("Blur Amount", Range(0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Blur;

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                // Mantenemos los vértices exactamente como están
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // Muestreo de 9 puntos (Kernel 3x3) para un blur más suave 
                // sin alterar la posición de los píxeles originales
                fixed4 col = tex2D(_MainTex, IN.texcoord) * 0.25;
                col += tex2D(_MainTex, IN.texcoord + float2(_Blur, 0)) * 0.125;
                col += tex2D(_MainTex, IN.texcoord - float2(_Blur, 0)) * 0.125;
                col += tex2D(_MainTex, IN.texcoord + float2(0, _Blur)) * 0.125;
                col += tex2D(_MainTex, IN.texcoord - float2(0, _Blur)) * 0.125;
                col += tex2D(_MainTex, IN.texcoord + float2(_Blur, _Blur)) * 0.0625;
                col += tex2D(_MainTex, IN.texcoord + float2(-_Blur, -_Blur)) * 0.0625;
                col += tex2D(_MainTex, IN.texcoord + float2(_Blur, -_Blur)) * 0.0625;
                col += tex2D(_MainTex, IN.texcoord + float2(-_Blur, _Blur)) * 0.0625;
                
                col.rgb *= IN.color.rgb;
                return col;
            }
            ENDCG
        }
    }
}
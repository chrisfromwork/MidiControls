Shader "UI/Unlit/Wave"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        _Highlight("Highlight", Color) = (0.95, 0.95, 0.95, 1)
        _Position("Position", Range(0, 1)) = 1.0
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
        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
			struct appdata_t
		{
			float4 vertex   : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex    : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord  : TEXCOORD0;
		};

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _Highlight;
		float _Position;

		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.texcoord = IN.texcoord;
			OUT.color = IN.color * _Color;
			return OUT;
		}

		fixed4 frag(v2f IN) : SV_Target
		{
			float2 uv = IN.texcoord.xy;
			float4 color = tex2D(_MainTex, uv);
			if (color.a > 0.1)
			{
				if (uv.x > _Position)
				{
					color = float4(0.282, 0.282, 0.282, 1.0);
				}
				else
				{
					color = _Highlight;
				}
			}

			return color;
		}
			ENDCG
        }
    }
}


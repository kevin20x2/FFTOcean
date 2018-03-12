Shader "Hidden/WhiteCapShader"
{
	Properties
	{
		_Resolution("Resolution",Float) = 512
		_Length("sea width", Float) = 512
		_Displacement ("Displacement",2D) = "black" {}
		_Bump("Bump",2D) = "bump" {}

	}
	SubShader
	{
		// No culling or depth

		Pass
		{
			Cull Off ZWrite Off ZTest Off
			ColorMask R
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			float _Resolution;
			float _Length;
			sampler2D _Displacement;
			sampler2D _Bump;

			float4 frag (v2f i) : SV_Target
			{
				float texelSize = 1 / _Length;
				float2 dDdy = 0.5f * (tex2D(_Displacement, i.uv + float4(0, -texelSize, 0, 0)).rb - tex2D(_Displacement, i.uv + float4(0, texelSize, 0, 0)).rb) / 4;
				float2 dDdx = 0.5f * (tex2D(_Displacement, i.uv + float4(-texelSize,0, 0, 0)).rb - tex2D(_Displacement, i.uv + float4( texelSize,0, 0, 0)).rb) / 4;
				float2 noise = 0.2 * tex2D(_Bump, i.uv).xz;
				float jacobian = (1 + dDdx.x) *(1 + dDdy.y) - dDdx.y * dDdy.x;
				float turb = max(0.0, 1 - jacobian + length(noise));
				//float turb = max(0.0, jacobian - length(noise));
				float xx;
				//float xx = max(0.0, 1 - jacobian + length(noise));
				//if(jacobian-length(noise) < 0.5)
					//return float4(1, 1, 1, 1);
				//xx = 1 + 3 * smoothstep(1.2, 1.8, turb);
//				xx = min(turb, 1);
				xx = smoothstep(0, 1, turb);
				return float4(xx, xx, xx, 1);
				//return float4(smoothstep(0, 1, 0.5 - jacobian + length(noise)), 0, 0, 1.0f);
				//return float4(0, 0, 0,1.0f);
			}
			ENDCG
		}
	}
}

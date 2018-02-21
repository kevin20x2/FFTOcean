Shader "Hidden/h0tShader"
{
	Properties
	{
		_CpuTime ("time ",Float)  = 0.0
		_InitTex ("Init Texture", 2D) = "white" {}
		_Resolution("resolution ", Float) = 256
		_WaveLength("wave length",Float ) = 256
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
#define G 9.8f
#define M_PI 3.141592653
			sampler2D _InitTex;
			float _CpuTime;
			float _Resolution;
			float _WaveLength;
			inline float getW(float2 k)
			{
				float w_0 = 2.0f *M_PI / 200.0f;
				float lenk = length(k);
				//return floor(sqrt(G * lenk)/w_0) *w_0;
				return sqrt(G* lenk*(1 + lenk * lenk / 370 / 370));
			}
			inline float2 GetK(float n, float m, float res,float len)
			{
				float w_0 = 2.0f *M_PI / 200.f;
				float x = 2*M_PI*(n - res / 2) / len;
				float y = 2*M_PI *(m - res / 2) / len;
				return float2(x, y);
			}
			inline float2 ComplexMult(float2 a, float2 b)
			{
				return float2(a.x*b.x-a.y*b.y,a.x*b.y+a.y*b.x);
			}
			float4 frag (v2f i) : SV_Target
			{
				float n = i.uv.x * _Resolution;
				float m = i.uv.y * _Resolution;
				float2 k = GetK(n, m, _Resolution,_WaveLength);
				float w = getW(k);
				float4 data = tex2D(_InitTex, i.uv);
				float2 wkt = float2 (cos(w*_CpuTime), sin(w*_CpuTime));
				float2 minuswkt = float2(cos(w*_CpuTime), -sin(w*_CpuTime));
				//fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				return float4(ComplexMult(data.rg,wkt) + ComplexMult(data.ba,minuswkt),0.0f,0.0f);
				
			}
			ENDCG
		}
	}
}

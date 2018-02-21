Shader "Hidden/H0Shader"
{
	Properties
	{
	//	_MainTex ("Texture", 2D) = "white" {}
		_Wind ("wind vector",Vector) = (1,1,0,0)
		_Resolution ("resolution" ,Float ) = 256
		_WaveLength ("wave length",Float ) = 256
		_Amplitude ("Phillips ampitude",Float) = 1
		_Random1 ("random1 ", Range(0,1)) = 0.1
		_Random2 ("random2 ",Range(0,1)) = 0.2
		_EPS ("eps" ,Range(0.000001,1)) = 0.2
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
			float2 _Wind;
			float _Resolution;
			float _Amplitude;
			float _Random1;
			float _Random2;
			float _EPS;
			float _WaveLength;

			#define M_G 9.8f
#define M_EPSILON 0.1
#define M_PI 3.141592653
			inline float UVRandom(float2 uv, float slat, float random)
			{
				uv += float2(slat, random);
				return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
			}
			float Phillips(float2 wind, float2 k)
			{
				float L = length(wind) * length(wind) / M_G;
				float klen = length(k);
				if (klen < _EPS) return 0;
				float k_dot_w = dot(normalize(k),normalize(wind));
				return _Amplitude*(512.0f*512.0f) /(_Resolution*_Resolution) * exp(-1.0f / ((klen * L)*(klen *L))) / (klen*klen*klen*klen) * (k_dot_w * k_dot_w);
			}
			float2 Geth0(float2 uv, float2 k, float rand1, float rand2)
			{
				float r1 = UVRandom(uv, 10.612, rand1);
				float r2 = UVRandom(uv, 11.899, rand2);

				r1 = clamp(r1, 0.01, 1);
				r2 = clamp(r2, 0.01, 1);

				float x = sqrt(-2 * log(r1));
				float y = 2 * M_PI * r2;
				
				float tt = sqrt(Phillips(_Wind, k) / 2);
				return float2(x * cos(y),x*sin(y))* tt;
			}
			float2 Conj(float2 x)
			{
				return float2(x.x, -x.y);
			}
			float2 GetK(float n, float m, float res,float len)
			{
				float x = 2*M_PI*(n - res / 2) / len;
				float y = 2*M_PI *(m - res / 2) / len;
				return float2(x, y);
			}
			

			float4 frag (v2f i) : SV_Target
			{
				float n = i.uv.x * _Resolution;
				float m = i.uv.y * _Resolution;
				float2 k = GetK(n, m ,_Resolution,_WaveLength);
				float2 h0 = Geth0(i.uv,k, _Random1, _Random2);

				float2 h0conj = Conj(Geth0(i.uv,-k, _Random1/2,_Random2*2));
				//float2 h0conj = Geth0(-k, _Random1,_Random2);
				//float2 h0conj = Conj(h0);
				return float4(h0,h0conj);
			}
			ENDCG
		}
	}
}

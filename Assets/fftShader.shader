Shader "Hidden/fftShader"
{
	Properties
	{ 
		_Input ("input sampler",2D) = "black" 
		_TransformSize ("Transform size",Float) = 256
		_SubTransformSize ("Log Size",Float) = 8
	}
	SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

		Pass
		{
			Cull Off
			ZWrite Off
			ZTest Off
			ColorMask RGBA
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "common.cginc"
			#pragma multi_compile _HORIZONTAL _VERTICAL

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
#define EPS 0.00001f
//#define PI 3.141592653f
			uniform sampler2D _Input;
			uniform float _TransformSize;
			uniform float _SubTransformSize;

			float2 MultComplex(float2 a, float2 b)
			{
				return float2(a.x*b.x - a.y*b.y, a.x*b.y+a.y*b.x);
			}
float4 frag (v2f i) : SV_Target
{
	float index;
#ifdef _HORIZONTAL
	index = i.uv.x *_TransformSize -0.5;
#else 
	index = i.uv.y *_TransformSize -0.5;
#endif
	float evenIndex = floor(index / _SubTransformSize) *(_SubTransformSize *0.5) + fmod(index, _SubTransformSize * 0.5) +0.5;

#ifdef _HORIZONTAL
	float4 even = tex2D(_Input, float2((evenIndex), i.uv.y * _TransformSize) / _TransformSize);
	float4 odd = tex2D(_Input, float2((evenIndex + _TransformSize * 0.5), i.uv.y* _TransformSize) / _TransformSize);
#else
	float4 even = tex2D(_Input, float2( i.uv.x * _TransformSize,(evenIndex)) / _TransformSize);
	float4 odd = tex2D(_Input, float2( i.uv.x* _TransformSize,(evenIndex + _TransformSize * 0.5)) / _TransformSize);
#endif

	float twiddleV = -2*PI*((index ) / _SubTransformSize);
	float2 twiddle = float2(cos(twiddleV), sin(twiddleV));
	float2 outputA = even.xy + MultComplex(twiddle, odd.xy);
	float2 outputB = even.zw + MultComplex(twiddle, odd.zw);
	//float res = _TransformSize * _TransformSize;
	//outputA = length(outputA) < EPS ? float2(0.0, 0.0) : outputA;
	//outputB = length(outputB) < EPS ? float2(0.0, 0.0) : outputB;

	return float4(outputA, outputB);

}
			ENDCG
		}
	}
}

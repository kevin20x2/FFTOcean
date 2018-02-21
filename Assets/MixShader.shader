Shader "Hidden/MixShader"
{
	Properties
	{
		_Input1("input1 Texture", 2D) = "black" {}
		_Input2("input2 Texture",2D) = "black" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off 
		//ZWrite Off 
		ZTest Always

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
			
			sampler2D _Input1;
			sampler2D _Input2;

			float4 frag (v2f i) : SV_Target
			{
				float4 data1 = tex2D(_Input1, i.uv);
				float4 data2 = tex2D(_Input2, i.uv);
				// just invert the colors
				return (data1+data2);
			}
			ENDCG
		}
	}
}

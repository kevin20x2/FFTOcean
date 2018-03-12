Shader "Hidden/WireFrameShader"
{
	Properties
	{
		_HeightInput ("Texture", 2D) = "black" {}
		_SpectrumInput("spectrum Texture",2D) = "black" {}
		_Diffuse("Diffuse",Color) = (0.5,0.65,0.75,1.0)
		_Specular("specular",Color) = (1,1,1,1)
		_Noise("noise ",2D) = "black" {}
		_WireColor("wirecolor",Color) = (0,0,0,0)
		_Gloss("gloss" , Int) = 2
		_Resolution("resoulution", Float) = 128
		_ShowWire("show Wire", Int) = 0
		_WhiteCapHeight("whitecap height",Float) = 1.0
	}
	SubShader
	{
			Tags { "RenderType" = "Opaque" }
			LOD 200
		// No culling or depth
				Pass
		{
			//Cull Off 
			//ZWrite On 
			//ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _HeightInput;
			sampler2D _SpectrumInput;
			sampler2D _Noise;
			float4 _Diffuse;
			float4 _Specular;
			float4 _WireColor;
			int _Gloss;
			int _ShowWire;
			float _Resolution;
			float _WhiteCapHeight;
			v2f vert (appdata v)
			{
				v2f o;
				float dy = tex2Dlod(_HeightInput,float4(v.uv,0.0f,0.0f)).r;
				float4 dxdz = tex2Dlod(_SpectrumInput, float4(v.uv, 0.0f, 0.0f));
				o.uv = v.uv;
				v.vertex.y +=dy;
				v.vertex.x -= dxdz.x;
				v.vertex.z -= dxdz.z;
				o.worldPos = mul(unity_WorldToObject,v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
#define EPS 0.00007f;

			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				//col = 1 - col;
				//float2 rg = tex2D(_HeightInput,i.uv).rg;
				float modx = fmod(i.uv.x , 1.0f / _Resolution);
				float mody = fmod(i.uv.y, 1.0f / _Resolution);
				//if (modx < EPS)
				//	return float4(0.0f, 0.0f, 0.0f, 1.0f);
				//if (mody < EPS)
				//	return float4(0.0f, 0.0f, 0.0f, 1.0f);
				float height_data = tex2D(_HeightInput,i.uv).r;
				float4 noise = tex2D(_Noise,i.uv);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 ambientColor = float3(0.0, 0.65, 0.75);
				float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				float4 t = tex2D(_SpectrumInput,i.uv);
				float3 nor = float3(0.0f, 1.0f, 0.0f) - float3(t.g,0.0f,t.a);
				float halfLambert = dot(normalize(nor), worldLightDir) *0.5 + 0.5;
				bool facing = dot(normalize(nor), worldLightDir) > 0.0;
				float3 diffuse = _LightColor0.rgb* _Diffuse.rgb * max(0.0f,dot(normalize(nor),worldLightDir));
				float3 reflectDir = normalize(reflect(-worldLightDir,normalize(nor)));
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				float3 specular = _LightColor0.rgb *_Specular.rgb*pow(saturate(dot(reflectDir, viewDir)), _Gloss);
				float fog_factor = min(-i.worldPos.z / 800.0, 1.0);
				float3 color = 0.3f*ambientColor + 0.3f*diffuse +(facing ? 1.8f*specular : float3(0, 0, 0));
				float3 fragColor = color * (1.0 - fog_factor) + float4(0.24, 0.75, 0.65, 1.0) * (fog_factor);

				bool wireframex = modx < EPS;
				bool wireframey = mody < EPS;
				bool wireframexy = abs(modx - mody) < EPS;
				if (height_data > _WhiteCapHeight)
					fragColor += noise;

				if (_ShowWire >0) {
					fragColor = wireframex ? _WireColor : fragColor;
					fragColor = wireframey ? _WireColor : fragColor;
					fragColor = wireframexy ? _WireColor : fragColor;
				}
				return float4(fragColor,1.0f);
			}
			ENDCG
		}
	}
}

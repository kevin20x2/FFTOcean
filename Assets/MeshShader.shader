Shader "Hidden/MeshShader"
{
	Properties
	{
		_HeightInput ("Texture", 2D) = "black" {}
		_SpectrumInput("spectrum Texture",2D) = "black" {}
		_WhiteCapInput("white cap ",2D) = "black" {}
		_Diffuse("Diffuse",Color) = (0.5,0.65,0.75,1.0)
		_Specular("specular",Color) = (1,1,1,1)
		_Gloss("gloss" , Int) = 2
		_WaveNumber("wave number",Int) = 3

	}
	SubShader
	{
			Tags { "RenderType" = "Opaque" }
			LOD 200
		// No culling or depth
				Pass
		{
			//Cull Off 
			//ZWrite Off 
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
			sampler2D _WhiteCapInput;
			float4 _Diffuse;
			float4 _Specular;
			int _Gloss;
			float _WaveNumber;
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
			

			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				//col = 1 - col;
				//float2 rg = tex2D(_HeightInput,i.uv).rg;
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 ambientColor = float3(0.0, 0.65, 0.75);
				float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				float4 t = tex2D(_SpectrumInput,i.uv);
				//float whitecap = saturate((1.0f-tex2D(_WhiteCapInput, i.uv).r));
				float whitecap = tex2D(_WhiteCapInput, i.uv).r;
				//whitecap = whitecap > 0.8 ? whitecap : 0.0f;
				float3 nor = float3(0.0f, 1.0f, 0.0f) - float3(t.g,0.0f,t.a);
				float halfLambert = dot(normalize(nor), worldLightDir) *0.5 + 0.5;
				bool facing = dot(normalize(nor), worldLightDir) > 0.0;
				float3 diffuse = _LightColor0.rgb* _Diffuse.rgb * max(0.0f,dot(normalize(nor),worldLightDir));
				float3 reflectDir = normalize(reflect(-worldLightDir,normalize(nor)));
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				float3 specular = _LightColor0.rgb *_Specular.rgb*pow(saturate(dot(reflectDir, viewDir)), _Gloss);
				float fog_factor = min(-i.worldPos.z / (64*15), 1.0);
			//	float fog_factor = 0.0f;
				float3 color = 0.3f*ambientColor + 0.3f*diffuse + (facing ? 1.8f*specular : float3(0, 0, 0)) + float3(whitecap,whitecap,whitecap);
				//float3 color = float3(whitecap, whitecap, whitecap);
				float3 fragColor = color *(1.0 - fog_factor) + float4(0.24, 0.75, 0.65, 1.0) * (fog_factor);
				return float4(fragColor,1.0f);
			}
			ENDCG
		}
	}
}

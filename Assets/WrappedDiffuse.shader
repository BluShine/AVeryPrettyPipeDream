Shader "Pipes/WrappedDiffuse" {
	Properties{
		_Bias("Bias", Range(1.0, 0)) = .5
		_Scale("Scale", Range(.5, 0)) = .5
		_Exponent("Exponent", Range(5, .1)) = 2
		_Specular("Specular", Range(1, 0.01)) = .5
		_SpecColor("Specular Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Light", Range(0, 10)) = 3
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf WrapLambert

		half _Bias;
		half _Scale;
		half _Exponent;
		half _Specular;
		half4 _SpecularColor;
		half _RimPower;
		half4 _RimColor;

		half4 LightingWrapLambert(SurfaceOutput s, half3 viewDir, UnityGI gi) {
			half3 h = normalize(gi.light.dir + viewDir);

			fixed diff = max(0, pow(dot(s.Normal, gi.light.dir) * _Scale + _Bias, _Exponent));

			float nh = max(0, dot(s.Normal, h));
			float spec = pow(nh, _Specular*128.0);

			half rim = 1 - saturate(dot(normalize(viewDir), s.Normal));

			fixed4 c;
			c.rgb = s.Albedo * gi.light.color * diff + gi.light.color * _SpecColor.rgb * spec + _RimColor.rgb * pow(rim, _RimPower);
			c.a = s.Alpha;

			return c;
		}

		inline void LightingWrapLambert_GI(
			SurfaceOutput s,
			UnityGIInput data,
			inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, s.Normal);
		}

		struct Input {
			float4 color: Color;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.color.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

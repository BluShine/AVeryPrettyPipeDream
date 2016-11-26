Shader "Pipes/Specular Vertex Color Ambient"
{
    Properties
    {
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_Glowyness("Glowyness", Range(0.01, 1)) = 0.078125
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM

        #pragma surface surf BlinnPhong

        half _Shininess;
		half _Glowyness;

        struct Input
        {
            float4 color: Color;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = IN.color.rgb;
            o.Alpha = IN.color.a;
            o.Gloss = IN.color.a;
            o.Specular = _Shininess;
			o.Emission = float3( IN.color.r * _Glowyness, IN.color.g * _Glowyness, IN.color.b * _Glowyness);
        }

        ENDCG
    }
    FallBack "Diffuse"
}

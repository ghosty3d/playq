Shader "Custom/DirectionalDissolve" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_HighlightColor("Highlight Color", Color) = (1,1,1,1)
        _HighlightPower("Highlight Power", Range(0.0, 1.0)) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _EffectTex ("Effect Texture (RGB)", 2D) = "white" {}
        _TextureBlend("Texture Blend", Range(0.0, 1.0)) = 0
        _DissolveAmount("Dissolve amount", Range(-3,3)) = 0
        _Direction("Direction", vector) = (0,1,0,0)
        [HDR]_Emission("Emission", Color) = (1,1,1,1)
        _EmissionThreshold("Emission threshold", float) = 0.1
        _NoiseSize("Noise size", float ) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" "DisableBatching" = "True"}
        LOD 200
        Cull off
        Lighting Off
 
        CGPROGRAM
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma surface surf Lambert vertex:vert
 
        sampler2D _MainTex;
        sampler2D _EffectTex;
        float _TextureBlend;
 
        struct Input {
            float2 uv_MainTex;
            float3 worldPosAdj;
        };
 
        fixed4 _Color;
        fixed4 _HighlightColor;
        float _HighlightPower;
        float _DissolveAmount;
        half4 _Direction;
        fixed4 _Emission;
        float _EmissionThreshold;
        float _NoiseSize;
 
        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.worldPosAdj =  mul (unity_ObjectToWorld, v.vertex.xyz);
        }
 
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
 
        float random (float2 input) { 
            return frac(sin(dot(input, float2(12.9898,78.233)))* 43758.5453123);
        }
 
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 cf = tex2D(_EffectTex, IN.uv_MainTex + _Time.y * 0.5);
            
            c = lerp(c, cf, _TextureBlend);
            
            //Clipping
            half test = (dot(IN.worldPosAdj, normalize(_Direction))) / 2;
            clip (test - _DissolveAmount);
            //Emission noise
            float squares = step(0.5, random(floor(IN.uv_MainTex * _NoiseSize) * _DissolveAmount));
            half emissionRing = step(test - _EmissionThreshold, _DissolveAmount) * squares;
 
            o.Albedo = lerp(c.rgb, _HighlightColor, _HighlightPower);
            o.Emission = lerp(lerp(c.rgb * 0.5, c.rgb, _TextureBlend), _HighlightColor, _HighlightPower);
            o.Emission += _Emission * emissionRing;
            o.Alpha = c.a; 
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Snow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SnowHeight("Snow Height Texture", 2D) = "white" {}
		_SnowThickness("Snow Thickness", Range(-2,2)) = 0.1
		_HeightScale("Height Scale", Range(0,2)) = 0.3
		_PlayArea("Play Area Size", Vector) = (1024,1024,0,0)
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:disp tessellate:tessFixed

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 5.0

		sampler2D _MainTex;
		sampler2D _SnowHeight;

		float4 tessFixed() {
			return 4;
		}
		struct appdata {

			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
			float2 texcoord : TEXCOORD0;
		};
        struct Input
        {
            float2 uv_MainTex;
			float4 col;
		};

        half _Glossiness;
        half _Metallic;
		float _SnowThickness;
		float _HeightScale;
		float4 _PlayArea;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling

		void disp(inout appdata v) {
		//	UNITY_INITIALIZE_OUTPUT(Input, o);

			//v.vertex.xyz += normalize(v.vertex.xyz); //v.normal * 3;
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float2 playAreaOffset = _PlayArea.zw * 0.5 - _PlayArea.xy;
			float3 uv = float3(playAreaOffset.x, 0, playAreaOffset.y) + worldPos;
			uv.x /= _PlayArea.z;
			uv.z /= _PlayArea.w;
			fixed4 height = tex2Dlod(_SnowHeight, float4(uv.x, uv.z, 0, 0));

			float3 worldNormal = UnityObjectToWorldNormal(v.normal);
			float snowOrNot = step(0, dot(worldNormal, float3(0, 1, 0)));
			float3 newPos = worldPos + (_SnowThickness * worldNormal + _HeightScale * height.x * float3(0, 1, 0)) * snowOrNot;
			v.vertex.xyz = mul(unity_WorldToObject, float4(newPos,1));
			//o.col = float4(0,1,1,1);// v.col.a = snowOrNot;
		}
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

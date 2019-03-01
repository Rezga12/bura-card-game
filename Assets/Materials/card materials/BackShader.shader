Shader "Custom/myShade" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SecondTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_CornerRadius("Corner Radius",Range(0,0.5)) = 0.0
		_Width("Width",float) = 1
		_Height("Height",float) = 1
		_GreenDelta("Green Delta",Range(-1,1)) = 0
		_RedDelta("Red Delta",Range(-1,1)) = 0
		_BlueDelta("Blue Delta",Range(-1,1)) = 0
		_IntensityFilter("Intensity Filter",Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SecondTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _CornerRadius;

		float _Width;
		float _Height;

		float _GreenDelta;
		float _RedDelta;
		float _BlueDelta;

		float _IntensityFilter;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			float scale = _Width/_Height;

			float x = IN.uv_MainTex.x;
			float y = IN.uv_MainTex.y;

			//float width = _Width

			if(c.g > _IntensityFilter){
				c.g = c.g + _GreenDelta;
				c.b = c.b + _BlueDelta;
				c.r = c.r + _RedDelta;
			}





			if(y < _CornerRadius && x*scale < _CornerRadius){
				if(pow(_CornerRadius - y,2) + pow(_CornerRadius - x*scale,2) > pow(_CornerRadius,2)){
					clip(-1);
				}

			}

			if(y < _CornerRadius && (1 - x)*scale < _CornerRadius){
				if(pow(_CornerRadius - y,2) + pow(_CornerRadius - (1 - x)*scale,2) > pow(_CornerRadius,2)){
					clip(-1);
				}
			}

			if(y > 1 - _CornerRadius && (1 - x)*scale < _CornerRadius){
				if(pow(y - 1 + _CornerRadius,2) + pow(_CornerRadius - (1 - x)*scale,2) > pow(_CornerRadius,2)){
					clip(-1);
				}
			}

			if(y > 1 -  _CornerRadius && x*scale < _CornerRadius){
				if(pow(y - 1 + _CornerRadius,2) + pow(_CornerRadius - x*scale,2) > pow(_CornerRadius,2)){
					clip(-1);
				}
			}


			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
			o.Albedo = _Color * c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
